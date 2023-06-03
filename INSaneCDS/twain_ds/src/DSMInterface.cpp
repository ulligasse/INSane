#include "DSMInterface.h"

#include <iostream>

#pragma unmanaged

using namespace std;

HMODULE gpDSM = 0;

TW_ENTRYPOINT g_DSM_Entry = 
{
  0,// TW_UINT32         Size;
  0,// DSMENTRYPROC      DSM_Entry;
  0,// DSM_MEMALLOCATE   DSM_MemAllocate;
  0,// DSM_MEMFREE       DSM_MemFree;
  0,// DSM_MEMLOCK       DSM_MemLock;
  0 // DSM_MEMUNLOCK     DSM_MemUnlock;
};

#ifdef TWH_CMP_GNU
  #include <dlfcn.h>
#endif

TW_UINT16 _DSM_Entry( pTW_IDENTITY _pOrigin,
                      pTW_IDENTITY _pDest,
                      TW_UINT32    _DG,
                      TW_UINT16    _DAT,
                      TW_UINT16    _MSG,
                      TW_MEMREF    _pData)
{
  TW_UINT16 ret = TWRC_FAILURE;

  if(0 == g_DSM_Entry.DSM_Entry)
  {
    #ifdef TWH_CMP_MSC
	    char DSMName[MAX_PATH];

	    memset(DSMName, 0, MAX_PATH*sizeof(char));

      if(GetWindowsDirectory (DSMName, MAX_PATH)==0)
      {
        DSMName[0]=0;
      }
#if (TWNDS_CMP_VERSION >= 1400)
	    if (DSMName[strlen(DSMName)-1] != '\\')
	    {
		    strcat_s(DSMName,MAX_PATH, "\\");
	    }		
	    strcat_s (DSMName,MAX_PATH, "TWAIN_32.dll");
#else
	    if (DSMName[strlen(DSMName)-1] != '\\')
	    {
		    strcat(DSMName, "\\");
	    }		
	    strcat(DSMName, "TWAIN_32.dll");
#endif

      if((0 == gpDSM) && !LoadDSMLib(DSMName))
      {
        cerr << "Could not load the DSM" << endl;
        return TWRC_FAILURE;
      }

      if(0 == g_DSM_Entry.DSM_Entry)
      {
        cerr << "No Entry Point for DSM_Entry" << endl;
        return TWRC_FAILURE;
      }
    #else
      cerr << "No Entry Point for DSM_Entry" << endl;
      return TWRC_FAILURE;
    #endif
  }

  // If we did not have an enty point before we do now.
  ret = g_DSM_Entry.DSM_Entry(_pOrigin, _pDest, _DG, _DAT, _MSG, _pData);
  return ret;
}

/////////////////////////////////////////////////////////////////////////////
bool LoadDSMLib(char* _pszLibName)
{
  // check if already opened
  if(0 != gpDSM)
  {
    return true;
  }

  memset(&g_DSM_Entry, 0, sizeof(TW_ENTRYPOINT));

#ifdef TWH_CMP_GNU
  char *error;
#endif //TWH_CMP_GNU

  if((gpDSM=LOADLIBRARY(_pszLibName)) != 0)
  {
    if((g_DSM_Entry.DSM_Entry=(DSMENTRYPROC)LOADFUNCTION(gpDSM, "DSM_Entry")) == 0)
    {
#ifdef TWH_CMP_MSC // dlsym returning NULL is not an error on Unix
      cerr << "Error - Could not find DSM_Entry function in DSM: " << _pszLibName << endl;
      return false; 
#endif //TWH_CMP_MSC
    }
#ifdef TWH_CMP_GNU
    if ((error = dlerror()) != 0)
    {
      cerr << "App - dlsym: " << error << endl;
      return false;
    }
#endif //TWH_CMP_GNU
  }
  else
  {
    cerr << "Error - Could not load DSM: " << _pszLibName << endl;
#ifdef TWH_CMP_GNU
    cerr << "App - dlopen: " << dlerror() << endl;
#endif //TWH_CMP_GNU
    return false;
  }


  return true;
}

/////////////////////////////////////////////////////////////////////////////
void unLoadDSMLib()
{
  if(gpDSM)
  {
    memset(&g_DSM_Entry, 0, sizeof(TW_ENTRYPOINT));
    UNLOADLIBRARY(gpDSM);
    gpDSM = 0;
  }
}

/////////////////////////////////////////////////////////////////////////////
void setEntryPoints(pTW_ENTRYPOINT _pEntryPoints)
{
  if(_pEntryPoints)
  {
    g_DSM_Entry = *_pEntryPoints;
  }
  else
  {
    memset(&g_DSM_Entry, 0, sizeof(TW_ENTRYPOINT));
  }
}

//////////////////////////////////////////////////////////////////////////////
// The following functions are defined in the DSM2,
// For backwards compatibiltiy on windows call the default function
TW_HANDLE _DSM_Alloc(TW_UINT32 _size)
{
  if(g_DSM_Entry.DSM_MemAllocate)
  {
    return g_DSM_Entry.DSM_MemAllocate(_size);
  }

#ifdef TWH_CMP_MSC
  return ::GlobalAlloc(GPTR, _size);
#endif

  return 0;
}

//////////////////////////////////////////////////////////////////////////////
void _DSM_Free(TW_HANDLE _hMemory)
{
  if(g_DSM_Entry.DSM_MemFree)
  {
    return g_DSM_Entry.DSM_MemFree(_hMemory);
  }

#ifdef TWH_CMP_MSC
  ::GlobalFree(_hMemory);
#endif

  return;
}

//////////////////////////////////////////////////////////////////////////////
TW_MEMREF _DSM_LockMemory(TW_HANDLE _hMemory)
{
  if(g_DSM_Entry.DSM_MemLock)
  {
    return g_DSM_Entry.DSM_MemLock(_hMemory);
  }

#ifdef TWH_CMP_MSC
  return (TW_MEMREF)::GlobalLock(_hMemory);
#endif

  return 0;
}

//////////////////////////////////////////////////////////////////////////////
void _DSM_UnlockMemory(TW_HANDLE _hMemory)
{
  if(g_DSM_Entry.DSM_MemUnlock)
  {
    return g_DSM_Entry.DSM_MemUnlock(_hMemory);
  }

#ifdef TWH_CMP_MSC
  ::GlobalUnlock(_hMemory);
#endif

  return;
}

//////////////////////////////////////////////////////////////////////////////
