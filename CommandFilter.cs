using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace ChrisLajoie.DupSelection
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("code")]
    [TextViewRole(PredefinedTextViewRoles.Editable)]
    class VsTextViewCreationListener : IVsTextViewCreationListener
    {
        [Import]
        IVsEditorAdaptersFactoryService AdaptersFactory = null;

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            var wpfTextView = AdaptersFactory.GetWpfTextView(textViewAdapter);
            if (wpfTextView == null)
            {
                Debug.Fail("Unable to get IWpfTextView from text view adapter");
                return;
            }

            CommandFilter filter = new CommandFilter(wpfTextView, GuidList.guidDupSelectionCmdSet);
            filter.RegisterCommandHandler(new DupSelectionCommand(wpfTextView));

            IOleCommandTarget next;
            if (ErrorHandler.Succeeded(textViewAdapter.AddCommandFilter(filter, out next)))
                filter.Next = next;
        }
    }

    class CommandFilter : IOleCommandTarget
    {
        IWpfTextView _view;
        Guid _cmdsetGuid;
        Dictionary<uint, CommandHandler> _commandMap;


        internal IOleCommandTarget Next { get; set; }


        public CommandFilter(IWpfTextView view, Guid cmdsetGuid)
        {
            _view = view;
            _cmdsetGuid = cmdsetGuid;
            _commandMap = new Dictionary<uint, CommandHandler>();
        }


        public void RegisterCommandHandler(CommandHandler handler)
        {
            _commandMap[handler.CommandID] = handler;
        }

        public void UnregisterCommandHandler(CommandHandler handler)
        {
            _commandMap.Remove(handler.CommandID);
        }


        public int QueryStatus(ref Guid pguidCmdGroup, uint cCmds, OLECMD[] prgCmds, IntPtr pCmdText)
        {
            if (pguidCmdGroup == _cmdsetGuid)
            {
                uint cmdID = prgCmds[0].cmdID;
                if (_commandMap.ContainsKey(cmdID))
                {
                    if (_commandMap[cmdID].IsCommandEnabled())
                        prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_ENABLED | OLECMDF.OLECMDF_SUPPORTED);
                    else
                        prgCmds[0].cmdf = (uint)(OLECMDF.OLECMDF_SUPPORTED);
                }

                return VSConstants.S_OK;
            }

            return Next.QueryStatus(ref pguidCmdGroup, cCmds, prgCmds, pCmdText);
        }

        public int Exec(ref Guid pguidCmdGroup, uint nCmdID, uint nCmdexecopt, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (pguidCmdGroup == _cmdsetGuid)
            {
                if (_commandMap.ContainsKey(nCmdID))
                    _commandMap[nCmdID].Execute();
                return VSConstants.S_OK;
            }

            return Next.Exec(ref pguidCmdGroup, nCmdID, nCmdexecopt, pvaIn, pvaOut);
        }
    }
}