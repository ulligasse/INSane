#include "CTWAINDS_Sample1.h"
#include <list>

#pragma unmanaged

#include "INSane_IDS_EntryWrapper.h"

typedef struct _DS_inst
{
  TW_IDENTITY AppId;
  CTWAINDS_Base *pDS; 
}DS_inst;

typedef list<DS_inst> lstDS;
lstDS g_lstDS;

typedef struct _DS_inst_VB
{
	TW_IDENTITY AppId;
	IDS_EntryWrapper *pDS;
} DS_inst_VB;

typedef list<DS_inst_VB> lstDS_VB;
lstDS_VB g_lstDS_VB;

#ifdef TWH_CMP_MSC
  HINSTANCE   g_hinstance     = 0;
#endif

#ifdef TWH_CMP_MSC
TW_UINT16 FAR PASCAL
#else
FAR PASCAL TW_UINT16 
#endif
DS_Entry( pTW_IDENTITY _pOrigin,
          TW_UINT32    _DG,
          TW_UINT16    _DAT,
          TW_UINT16    _MSG,
          TW_MEMREF    _pData)
{

  IDS_EntryWrapper* pTWAIN_VB = 0;
  if(_pOrigin)
  {
    lstDS_VB::iterator llIter=g_lstDS_VB.begin();
    for(;llIter!=g_lstDS_VB.end();llIter++)
    {
      if((*llIter).AppId.Id==_pOrigin->Id)
      {
        pTWAIN_VB=(*llIter).pDS;
      }
    }
  }
  
  // Curently we are not open
  if( 0 == pTWAIN_VB )
  {
	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
	pTWAIN_VB = IDS_EntryWrapper::CreateInstance();
	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    if( NULL == pTWAIN_VB )
    {
      return TWRC_FAILURE;
	}

	DS_inst_VB _DS_VB;
	_DS_VB.pDS = pTWAIN_VB;
	if (_pOrigin) {
		_DS_VB.AppId = *_pOrigin;
	}

	g_lstDS_VB.push_back(_DS_VB);
  }
  // If we were not open before, we are now, so continue with the TWAIN call
  TW_INT16 result = pTWAIN_VB->Message_To_DS(_pOrigin, _DG, _DAT, _MSG, _pData);

  if( TWRC_SUCCESS == result && 
      DG_CONTROL == _DG && DAT_IDENTITY == _DAT && MSG_CLOSEDS == _MSG &&
      NULL != pTWAIN_VB )
  {
    lstDS_VB::iterator llIter=g_lstDS_VB.begin();
    for(;llIter!=g_lstDS_VB.end();)
    {
      if((*llIter).AppId.Id==_pOrigin->Id)
      {
        delete  (*llIter).pDS;
        llIter = g_lstDS_VB.erase(llIter);

        continue;
      }
      llIter++;
    }
  }


  return result;
}

//////////////////////////////////////////////////////////////////////////////


#ifdef TWH_CMP_MSC
BOOL WINAPI DllMain(HINSTANCE _hmodule,
                    DWORD     _dwReasonCalled,
                    LPVOID)
{
  switch (_dwReasonCalled)
  {
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
      break;
    case DLL_PROCESS_ATTACH:
      g_hinstance = _hmodule;
      break;
    case DLL_PROCESS_DETACH:
      unLoadDSMLib();
      g_hinstance = 0;
      break;
  }
  return(TRUE);
}
#elif (TWNDS_CMP == TWNDS_CMP_GNUGPP)
    // Nothing for us to do...
#else
    #error Sorry, we do not recognize this system...
#endif