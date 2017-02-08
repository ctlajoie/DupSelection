# Duplicate Selection

This Visual Studio extension adds a "Duplicate Selection" command that you can bind to a keyboard shortcut of your choosing (but I recommend Ctrl+D).
When invoked, the command inserts whatever text you have selected in front of the current selection.
Alternatively, if you do not have a selection, it duplicates the line the caret is on.

## Key Binding
To change the keyboard shortcut that the command is bound to, go into _Tools_ > _Options_ > _Keyboard_, and type "Duplicate" 
in the search box (the full command string is "Edit.DuplicateSelection"). Here you can bind it to any shortcut in the 
same way you would for any other command.


## Examples / Screenshots
Below are some before and after screenshots to help illustrate what the command will do in different situations.

| Before  | After |
| --------| ------|
| ![No Selection Before](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/ns-before.png)  | ![No Selection After](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/ns-after.png)  |
| ![Multiline Stream Before](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/ml-stream-before.png)  | ![Multiline Stream After](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/ml-stream-after.png)  |
| ![Single Line Stream Before](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/sl-stream-before.png)  | ![Single Line Stream After](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/sl-stream-after.png)  |
| ![Block Before](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/blk-before.png)  | ![Block After](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/blk-after.png)  |

If you select **multiple lines** but the selection does **not** encompass each line entirely, all lines that your selection touches will be duplicated.

| Before  | After |
| --------| ------|
| ![Partial Multiline Stream Before](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/mlp-stream-before.png)  | ![Partial Multiline Stream After](https://raw.githubusercontent.com/ctlajoie/DupSelection/master/screenshots/mlp-stream-after.png)  |
