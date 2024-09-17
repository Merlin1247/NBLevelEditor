namespace BreakoutNESLevelEditor;

public class SelectBlockType : Form
{
    private Editor? editor;

    public void GetEditor(Editor sender)
    {
        editor = sender;
    }

    public SelectBlockType()
    {
        this.AutoScaleMode = AutoScaleMode.Font;
        this.ClientSize = new Size(400, 200);
        this.Text = "Select Element Type...";
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.BackColor = Color.FromArgb(255, 40, 40, 40);
        this.MaximizeBox = false;

        singleBtn = new Button(){
            Size = new Size(150, 25),
            Location = new Point(5, 5),
            Text = "Single block",
            BackColor = Color.White
        };
        this.Controls.Add(singleBtn);
        singleBtn.Click += Single;

        lineBtn = new Button(){
            Size = new Size(150, 25),
            Location = new Point(5, 35),
            Text = "Line of blocks",
            BackColor = Color.White
        };
        this.Controls.Add(lineBtn);
        lineBtn.Click += BasicLine;

        boolLineBtn = new Button(){
            Size = new Size(150, 25),
            Location = new Point(5, 65),
            Text = "Line of blocks(boolean)",
            BackColor = Color.White,
            Enabled = false
        };
        this.Controls.Add(boolLineBtn);
        boolLineBtn.Click += DuetLine;

        description = new Label(){
            Size = new Size(150, 150),
            Location = new Point(160, 10),
            Text = "One lonely block...\n\nA whole row of blocks!\n\nUnimplemented",
            ForeColor = Color.White
        };
        this.Controls.Add(description);
    }

    public void Single(object? sender, EventArgs? e)
    {
        if(editor != null)
        {
            editor.FinishBlockCreation(0);
        }
        this.Close();
    }
    public void BasicLine(object? sender, EventArgs? e)
    {
        if(editor != null)
        {
            editor.FinishBlockCreation(1);
        }
        this.Close();
    }
    public void DuetLine(object? sender, EventArgs? e)
    {
        if(editor != null)
        {
            editor.FinishBlockCreation(2);
        }
        this.Close();
    }

    private System.Windows.Forms.Button singleBtn;
    private System.Windows.Forms.Button lineBtn;
    private System.Windows.Forms.Button boolLineBtn;
    private System.Windows.Forms.Label description;
}