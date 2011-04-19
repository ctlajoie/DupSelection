using System;
using System.Linq;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;

namespace ChrisLajoie.DupSelection
{
    class DupSelectionCommand : CommandHandler
    {
        public override uint CommandID
        {
            get { return PkgCmdIDList.cmdidDupSelection; }
        }


        public DupSelectionCommand(IWpfTextView view) : base(view) { }


        public override bool IsCommandEnabled()
        {
            return true;
        }

        public override void Execute()
        {
            ITextSnapshot snapshot = _view.TextSnapshot;

            if (snapshot != snapshot.TextBuffer.CurrentSnapshot)
                return;

            if (_view.Selection.IsEmpty)
            {
                // nothing is selected, duplicate current line
                using (var edit = snapshot.TextBuffer.CreateEdit())
                {
                    ITextSnapshotLine line = snapshot.GetLineFromPosition(_view.Selection.AnchorPoint.Position.Position);
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
                if (_view.Selection.Mode == TextSelectionMode.Stream)
                {
                    var startLine = snapshot.GetLineFromPosition(_view.Selection.Start.Position.Position);
                    var endLine = snapshot.GetLineFromPosition(_view.Selection.End.Position.Position);
                    if (startLine.LineNumber != endLine.LineNumber)
                    {
                        // selection spans multiple lines
                        var newSelStart = _view.Selection.Start.Position;
                        var newSelEnd = _view.Selection.End.Position;

                        if (startLine.Start < newSelStart)
                            newSelStart = startLine.Start;

                        if (endLine.Start != newSelEnd)
                            newSelEnd = endLine.EndIncludingLineBreak;

                        if (_view.Selection.Start.Position != newSelStart || _view.Selection.End.Position != newSelEnd)
                        {
                            _view.Selection.Select(new SnapshotSpan(newSelStart, newSelEnd), false);
                            _view.Caret.MoveTo(newSelEnd, PositionAffinity.Predecessor);
                        }
                    }
                }

                // When text is inserted into a pre-existing selection, VS extends the selection
                // to also contain the inserted text. This is not desired in this case, so save
                // the current selection so we can revert to it later.
                var initAnchor = _view.Selection.AnchorPoint;
                var initActive = _view.Selection.ActivePoint;

                using (var edit = snapshot.TextBuffer.CreateEdit())
                {
                    // Unless this is a box selection there will likely only be one span.
                    // Iterate backwards over the spans so we don't have to change the insertion point
                    // to compensate for already-inserted text.
                    foreach (var span in _view.Selection.SelectedSpans.Reverse())
                    {
                        if (!span.IsEmpty)
                        {
                            edit.Insert(span.End.Position, span.GetText());
                        }
                    }
                    edit.Apply();
                }

                var newAnchor = initAnchor.TranslateTo(_view.TextSnapshot, PointTrackingMode.Negative);
                var newActive = initActive.TranslateTo(_view.TextSnapshot, PointTrackingMode.Negative);
                _view.Selection.Select(newAnchor, newActive);
                _view.Caret.MoveTo(newActive, PositionAffinity.Predecessor);
            }
        }
    }
}
