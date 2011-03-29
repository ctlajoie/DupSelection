using System;
using Microsoft.VisualStudio.Text.Editor;

namespace ChrisLajoie.DupSelection
{
    abstract class CommandHandler
    {
        protected IWpfTextView _view;


        abstract public uint CommandID { get; }


        public CommandHandler(IWpfTextView view)
        {
            _view = view;
        }


        abstract public bool IsCommandEnabled();

        abstract public void Execute();
    }
}
