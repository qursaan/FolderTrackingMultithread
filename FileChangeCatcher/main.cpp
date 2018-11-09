#include <iostream>
#include <windows.h>
#include <Winbase.h>
#include <stdlib.h>
#include <stdio.h>
#include <tchar.h>
#include <pthread.h>
#include <fstream>
#include <string.h>

using namespace std;

#define MAX_DIRS    25
#define MAX_FILES   255
#define MAX_BUFFER  4096
#define BUFSIZE 4096
#define PIPE_TIMEOUT 1000
#define NUM_THREADS 100

extern "C" {
    WINBASEAPI BOOL WINAPI
    ReadDirectoryChangesW( HANDLE hDirectory, LPVOID lpBuffer,
                           DWORD nBufferLength, BOOL bWatchSubtree,
                           DWORD dwNotifyFilter,
                           LPDWORD lpBytesReturned,
                           LPOVERLAPPED lpOverlapped,
                           LPOVERLAPPED_COMPLETION_ROUTINE lpCompletionRoutine);
}

typedef struct _THREAD_INFO
{
    char *thread_id;
    char *path;
} THREAD_INFO;

typedef struct _DIRECTORY_INFO
{
    HANDLE hDir;
    TCHAR lpszDirName[MAX_PATH];
    CHAR lpBuffer[MAX_BUFFER];
    DWORD dwBufLength;
    OVERLAPPED Overlapped;
} DIRECTORY_INFO, *PDIRECTORY_INFO, *LPDIRECTORY_INFO;

DIRECTORY_INFO DirInfo[MAX_DIRS];
TCHAR FileList[MAX_FILES*MAX_PATH];
DWORD numDirs;

VOID WatchDirectory(THREAD_INFO* mythread)
{
    char buf[2048];
    DWORD nRet;
    char filename[MAX_PATH];
    BOOL result         = TRUE;
    BOOL flg            = FALSE;
    HANDLE hPipe        = NULL;
    char pip_name[100];
    char directory_path[MAX_PATH];
    DWORD cbReplyBytes  = 0, cbWritten = 0;
    HANDLE hHeap        = GetProcessHeap();
    TCHAR* pchReply   = (TCHAR*)HeapAlloc(hHeap, 0, BUFSIZE*sizeof(TCHAR));
    if (pchReply == NULL)
    {
        printf( "\nERROR - Pipe Server Failure:\n");
        printf( "   Thread got an unexpected NULL heap allocation.\n");
        printf( "   Thread exitting.\n");
        return;
    }
    printf("Starting watching folder @%s\n",mythread->path);


    sprintf(directory_path,"%s",mythread->path);
    directory_path[strlen(directory_path)-2]='\0';

    sprintf(pip_name, "\\\\.\\pipe\\qursaan%s", mythread->thread_id);

    hPipe = CreateNamedPipe (pip_name,
                             PIPE_ACCESS_DUPLEX,
                             PIPE_TYPE_MESSAGE |
                             PIPE_READMODE_MESSAGE |
                             PIPE_WAIT,
                             PIPE_UNLIMITED_INSTANCES,
                             BUFSIZE,
                             BUFSIZE,
                             PIPE_TIMEOUT,
                             NULL);

    if (hPipe == INVALID_HANDLE_VALUE)
    {
        printf("CreateFile failed for Named Pipe client\n" );
        return ;
    }

    ConnectNamedPipe(hPipe, NULL);
    printf("Success Create thread for client #%s\n", mythread->thread_id);


    DirInfo[0].hDir = CreateFileA(directory_path,
                                  GENERIC_READ|FILE_LIST_DIRECTORY,
                                  FILE_SHARE_READ|FILE_SHARE_WRITE|FILE_SHARE_DELETE,
                                  NULL,
                                  OPEN_EXISTING,
                                  FILE_FLAG_BACKUP_SEMANTICS|FILE_FLAG_OVERLAPPED,
                                  NULL);

    if(DirInfo[0].hDir == INVALID_HANDLE_VALUE)
    {
        return;
    }

    lstrcpyA ( DirInfo[0].lpszDirName, directory_path);
    OVERLAPPED PollingOverlap;

    FILE_NOTIFY_INFORMATION* pNotify;
    int offset;
    PollingOverlap.OffsetHigh = 0;
    PollingOverlap.hEvent = CreateEvent(NULL,TRUE,FALSE,NULL);

    while(result)
    {
        result = ReadDirectoryChangesW(
                     DirInfo[0].hDir,
                     &buf,
                     sizeof(buf),
                     TRUE,
                     FILE_NOTIFY_CHANGE_FILE_NAME |
                     FILE_NOTIFY_CHANGE_DIR_NAME |
                     FILE_NOTIFY_CHANGE_SIZE,
                     &nRet,
                     &PollingOverlap,
                     NULL);

        WaitForSingleObject(PollingOverlap.hEvent,INFINITE);
        offset = 0;
        do
        {
            pNotify = (FILE_NOTIFY_INFORMATION*)((char*)buf + offset);
            strcpy(filename, "");
            int filenamelen = WideCharToMultiByte(CP_ACP, 0, pNotify->FileName, pNotify->FileNameLength/2, filename, sizeof(filename), NULL, NULL);
            filename[pNotify->FileNameLength/2] = '\0';
            switch(pNotify->Action)
            {
            case FILE_ACTION_ADDED:
                sprintf(pchReply,"ADD: \t%s",filename);
                break;
            case FILE_ACTION_REMOVED:
                sprintf(pchReply,"REMOVED: \t%s",filename);
                break;
            case FILE_ACTION_MODIFIED:
                sprintf(pchReply,"MODIFIED: \t%s",filename);
                break;
            case FILE_ACTION_RENAMED_OLD_NAME:
                sprintf(pchReply,"RENAMED_FROM: \t%s",filename);
                break;
            case FILE_ACTION_RENAMED_NEW_NAME:
                sprintf(pchReply,"RENAMED_TO: \t%s",filename);
                break;
            default:
                sprintf(pchReply,"ERROR\t");
                break;
            }
            //send
            cbReplyBytes = strlen(pchReply);
            flg = WriteFile(hPipe, pchReply, cbReplyBytes, &cbWritten, NULL);
            if (!flg || cbReplyBytes != cbWritten)
            {
                printf("Fail write to client\n");
                CloseHandle( DirInfo[0].hDir );
            }
            offset += pNotify->NextEntryOffset;
        }
        while(pNotify->NextEntryOffset);
    }
    CloseHandle( DirInfo[0].hDir );
}

VOID *CatchDir(VOID *threadarg)
{
    THREAD_INFO *my_data;
    my_data = (THREAD_INFO *) threadarg;
    WatchDirectory(my_data);
    pthread_exit(NULL);
    return 0;
}

DWORD StartPpServer()
{
    BOOL   fConnected = FALSE;
    DWORD  dwThreadId = 0;
    HANDLE hPipe = INVALID_HANDLE_VALUE;
    LPTSTR lpszPipename = TEXT("\\\\.\\pipe\\qursaan");

    BOOL flg            = FALSE;
    DWORD cbBytesRead = 0, cbReplyBytes = 0, cbWritten = 0;
    HANDLE hHeap      = GetProcessHeap();
    TCHAR* pchRequest = (TCHAR*)HeapAlloc(hHeap, 0, BUFSIZE*sizeof(TCHAR));
    TCHAR* pchReply   = (TCHAR*)HeapAlloc(hHeap, 0, BUFSIZE*sizeof(TCHAR));

    pthread_t threads[NUM_THREADS];
    THREAD_INFO tt[NUM_THREADS];
    LPTSTR _MSG_NEW = TEXT("NEW");
    //LPTSTR _MSG_DEL = "DEL";
    LPTSTR _MSG_PIP ;
    LPTSTR _MSG_DIR ;
    INT hThread;

    pthread_attr_t attr;

    hThread = pthread_attr_init(&attr);
    if (hThread == -1)
    {
        perror("error in pthread_attr_init");
        exit(1);
    }
    pthread_attr_setdetachstate(&attr, PTHREAD_CREATE_JOINABLE);

    if (pchRequest == NULL)
    {
        printf( "\nERROR - Pipe Server Failure:\n");
        printf( "   MainThread got an unexpected NULL heap allocation.\n");
        printf( "   MainThread exitting.\n");
        if (pchReply != NULL)
            HeapFree(hHeap, 0, pchReply);
        return (DWORD)-1;
    }

    if (pchReply == NULL)
    {
        printf( "\nERROR - Pipe Server Failure:\n");
        printf( "   MainThread got an unexpected NULL heap allocation.\n");
        printf( "   MainThread exitting.\n");
        if (pchRequest != NULL)
            HeapFree(hHeap, 0, pchRequest);
        return (DWORD)-1;
    }

    _tprintf(TEXT("Server: MainThread Awaiting Client Connection\n"));
    hPipe = CreateNamedPipe(
                lpszPipename,
                PIPE_ACCESS_DUPLEX,
                PIPE_TYPE_MESSAGE |
                PIPE_READMODE_MESSAGE |
                PIPE_WAIT,
                PIPE_UNLIMITED_INSTANCES,
                BUFSIZE,
                BUFSIZE,
                0,
                NULL);

    if (hPipe == INVALID_HANDLE_VALUE)
    {
        _tprintf(TEXT("Create Server failed\n"));
        (DWORD)-1;
    }
    fConnected = ConnectNamedPipe(hPipe, NULL) ? TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);

    if (fConnected)
    {
        printf("MainThread created, receiving and processing messages.\n");
        printf("Client Connected, Wait Tracking process..\n");

        //DO PROCESS
        INT ti = 0;
        INT seq = 0;
        while (1)
        {
            if(ti == 0) //send
            {
                strcpy( pchReply,"#123");
                cbReplyBytes = strlen(pchReply);//+1)*sizeof(TCHAR);
                //Write status to Client
                flg = WriteFile(hPipe, pchReply, cbReplyBytes, &cbWritten, NULL);
                if (!flg || cbReplyBytes != cbWritten)
                {
                    printf("Fail write to client\n");
                }
                else
                {
                    ti++;
                    printf("Waiting Main Client Order...\n");
                }
            }
            else if(ti==1)  //receive
            {
                flg = ReadFile( hPipe,  pchRequest, BUFSIZE*sizeof(TCHAR), &cbBytesRead,NULL);
                if (!flg || cbBytesRead == 0)
                {
                    if (GetLastError() == ERROR_BROKEN_PIPE)
                    {
                        _tprintf(TEXT("MainThread: main client disconnected.\n"));
                    }
                    else
                    {
                        _tprintf(TEXT("MainThread: Read Request failed.\n"));
                    }
                    break;
                }
                else
                {
                    pchRequest[cbBytesRead]='\0';
                }
                printf( "Client Request>> ");

                //HANDLE REQUETS
                if(seq == 0 && strncmp(pchRequest,_MSG_NEW,strlen(_MSG_NEW))==0)
                {
                    printf("New Tracking Request\n");
                    seq++;
                }
                else if(seq == 1)
                {
                    _MSG_PIP= pchRequest;
                    printf("#ID: ");
                    printf("%s", _MSG_PIP );
                    seq++;
                }
                else if(seq == 2)
                {
                    _MSG_DIR= pchRequest;
                    printf("#URL: ");
                    printf("%s", _MSG_DIR );
                    seq=0;
                    //start thread
                    char buffer [100];
                    sprintf(buffer, "%d", dwThreadId);
                    tt[dwThreadId].thread_id = buffer;
                    tt[dwThreadId].path = _MSG_DIR;

                    hThread = pthread_create(&threads[dwThreadId], &attr, CatchDir, (VOID*)&tt[dwThreadId]);

                    if (hThread)
                    {
                        _tprintf(TEXT("CreateThread failed\n"));
                        //return (DWORD)-1;
                    }
                    else
                    {
                        _tprintf(TEXT("CreateThread\n"));
                        dwThreadId++;
                    }

                }
            }
        }
    }
    else
    {
        FlushFileBuffers(hPipe);
        DisconnectNamedPipe(hPipe);
        CloseHandle(hPipe);
        pthread_exit(NULL);
        _tprintf( TEXT("Main Thread exiting.\n"));
        return (DWORD)0;
    }
    return(DWORD)0;
}


int main ()
{
    printf( " _______  __   __  ______    _______  _______  _______  __    _ \n"
            "|       ||  | |  ||    _ |  |       ||   _   ||   _   ||  |  | |\n"
            "|   _   ||  | |  ||   | ||  |  _____||  |_|  ||  |_|  ||   |_| |\n"
            "|  | |  ||  |_|  ||   |_||_ | |_____ |       ||       ||       |\n"
            "|  |_|  ||       ||    __  ||_____  ||       ||       ||  _    |\n"
            "|      | |       ||   |  | | _____| ||   _   ||   _   || | |   |\n"
            "|____||_||_______||___|  |_||_______||__| |__||__| |__||_|  |__|\n\n");

    printf("Starting application\n");
    printf("Staring Main Server\n");


    StartPpServer();
    return 0;
}



