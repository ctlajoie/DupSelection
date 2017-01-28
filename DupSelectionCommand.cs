using System;
using System.Linq;
using System.ComponentModel.Design;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System.Diagnostics;

namespace ChrisLajoie.DupSelection
{
    class DupSelectionCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("23997e47-6cf3-4df3-ae8a-a51f95a25312");

        private readonly Package _package;

        public static DupSelectionCommand Instance
        {
            get;
            private set;
        }

        private IServiceProvider ServiceProvider
        {
            get { return this._package; }
        }


        private DupSelectionCommand(Package package)
        {
            this._package = package ?? throw new ArgumentNullException(nameof(package));

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(Constants.guidDupSelectionCmdSet, Constants.cmdidDupSelection);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        public static void Initialize(Package package)
        {
            Instance = new DupSelectionCommand(package);
        }

        private void MenuItemCallback(object sender, EventArgs e)
        {
            IWpfTextView textview = Helpers.GetCurentTextView();

            if (textview == null)
            {
                Debug.WriteLine("Could not find IWpfTextView");
                return;
            }

            ITextSnapshot snapshot = textview.TextSnapshot;

            if (snapshot != snapshot.TextBuffer.CurrentSnapshot)
                return;

            if (textview.Selection.IsEmpty)
            {
                // nothing is selected, duplicate current line
                using (var edit = snapshot.TextBuffer.CreateEdit())
                {
                    ITextSnapshotLine line = snapshot.GetLineFromPosition(textview.Selection.AnchorPoint.Position.Position);
                    edit.Insert(line.EndIncludingLineBreak.Position, line.GetTextIncludingLineBreak());
                    edit.Apply();
                }
            }
            else
            {
                // duplicate selection

                // If we have a multi-line stream slection, it is likely that the user wants to
                // duplicate all lines in the selection. Extend the selection to accomplish this
                // if necessary.
                if (textview.Selection.Mode == TextSelectionMode.Stream)
                {
                    var startLine = snapshot.GetLineFromPosition(textview.Selection.Start.Position.Position);
                    var endLine = snapshot.GetLineFromPosition(textview.Selection.End.Position.Position);
                    if (startLine.LineNumber != endLine.LineNumber &&
                        (!textview.Selection.IsReversed || textview.Selection.End.Position != endLine.End))
                    {
                        // selection spans multiple lines
                        var newSelStart = textview.Selection.Start.Position;
                        var newSelEnd = textview.Selection.End.Position;
                        if (startLine.Start < newSelStart)
                            newSelStart = startLine.Start;
                        if (endLine.Start != newSelEnd)
                            newSelEnd = endLine.EndIncludingLineBreak;
                        if (textview.Selection.Start.Position != newSelStart || textview.Selection.End.Position != newSelEnd)
                        {
                            textview.Selection.Select(new SnapshotSpan(newSelStart, newSelEnd), false);
                            textview.Caret.MoveTo(newSelEnd, PositionAffinity.Predecessor);
                        }
                    }
                }

                // When text is inserted into a pre-existing selection, VS extends the selection
                // to also contain the inserted text. This is not desired in this case, so save
                // the current selection so we can revert to it later.
                var initAnchor = textview.Selection.AnchorPoint;
                var initActive = textview.Selection.ActivePoint;

                using (var edit = snapshot.TextBuffer.CreateEdit())
                {
                    // Unless this is a box selection there will likely only be one span.
                    // Iterate backwards over the spans so we don't have to change the insertion point
                    // to compensate for already-inserted text.
                    foreach (var span in textview.Selection.SelectedSpans.Reverse())
                    {
                        if (!span.IsEmpty)
                        {
                            edit.Insert(span.End.Position, span.GetText());
                        }
                    }
                    edit.Apply();
                }

                var newAnchor = initAnchor.TranslateTo(textview.TextSnapshot, PointTrackingMode.Negative);
                var newActive = initActive.TranslateTo(textview.TextSnapshot, PointTrackingMode.Negative);
                textview.Selection.Select(newAnchor, newActive);
                textview.Caret.MoveTo(newActive, PositionAffinity.Predecessor);
            }
        }
    }
}
