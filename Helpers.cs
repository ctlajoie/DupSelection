using System;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.ComponentModelHost;

namespace ChrisLajoie.DupSelection
{
    static class Helpers
    {
        public static IWpfTextView GetCurentTextView()
        {
            var componentModel = GetComponentModel();
            if (componentModel == null) return null;
            var vsTextView = GetCurrentNativeTextView();
            if (vsTextView == null) return null;
            var editorAdapter = componentModel.GetService<IVsEditorAdaptersFactoryService>();
            return editorAdapter.GetWpfTextView(vsTextView);
        }

        public static IVsTextView GetCurrentNativeTextView()
        {
            var textManager = (IVsTextManager)ServiceProvider.GlobalProvider.GetService(typeof(SVsTextManager));

            IVsTextView activeView = null;
            textManager.GetActiveView(1, null, out activeView);
            return activeView;
        }

        public static IComponentModel GetComponentModel()
        {
            return (IComponentModel)Package.GetGlobalService(typeof(SComponentModel));
        }
    }
}
