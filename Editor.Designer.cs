using System.Drawing.Drawing2D;

namespace BreakoutNESLevelEditor;

partial class Editor
{
    /// <summary>
    ///  Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    #region Windows Form Designer generated code (not)

    /// <summary>
    ///  Required method for Designer support - do not modify
    ///  the contents of this method with the code editor.
    ///  Jk - do whatever you want, the designer isn't in VS code
    /// </summary>
    private void InitializeComponent()
    {
        this.components = new System.ComponentModel.Container();
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new Size(900, 525);
        this.Text = "Merlin_1247 NES Breakout Level Editor";
        this.FormBorderStyle = FormBorderStyle.FixedSingle;
        this.BackColor = Color.FromArgb(255, 40, 40, 40);
        this.MaximizeBox = false;

        levelSelect = new ComboBox(){
            Size = new Size(100, 20),
            Location = new Point(8, 8),
            DropDownStyle = ComboBoxStyle.DropDownList
        };
        levelSelect.Items.Add(" ");
        this.Controls.Add(levelSelect);

        ballImage = new PictureBoxWithExtraSettings(){
            Size = new Size(10, 10),
            Location = new Point(8, 36),
            SizeMode = PictureBoxSizeMode.StretchImage,
            ImageLocation = @"Images\ball.png",
            InterpolationMode = InterpolationMode.NearestNeighbor,
            PixelOffsetMode = PixelOffsetMode.Half
        };
        this.Controls.Add(ballImage);

        gameBackground = new PictureBoxWithExtraSettings(){
            Size = new Size(512, 480),
            Location = new Point(8, 36),
            SizeMode = PictureBoxSizeMode.StretchImage,
            ImageLocation = @"Images\background.png",
            InterpolationMode = InterpolationMode.NearestNeighbor,
            PixelOffsetMode = PixelOffsetMode.Half
        };
        this.Controls.Add(gameBackground);
        
        saveToFile = new Button(){
            Size = new Size(100, 25),
            Location = new Point(113, 7),
            Text = "Save",
            BackColor = Color.White
        };
        this.Controls.Add(saveToFile);

        loadFromFile = new Button(){
            Size = new Size(100, 25),
            Location = new Point(218, 7),
            Text = "Load",
            BackColor = Color.White
            
        };
        this.Controls.Add(loadFromFile);

        propertiesTab = new TabControl(){
            Size = new Size(362, 508),
            Location = new Point(528, 8),
        };
        this.Controls.Add(propertiesTab);

        Color tabColour = Color.FromArgb(255, 60, 60, 60);

        infoTab = new TabPage(){
            Text = "Info",
            BackColor = tabColour
        };

        ballTab = new TabPage(){
            Text = "Ball",
            BackColor = tabColour
        };

        blocksTab = new TabPage(){
            Text = "Blocks",
            BackColor = tabColour
        };

        bytesTab = new TabPage(){
            Text = "Raw Data",
            BackColor = tabColour
        };

        propertiesTab.Controls.AddRange(new Control[] {infoTab, ballTab, blocksTab, bytesTab});

        bytesText = new Label(){
            Size = new Size(350, 500),
            Location = new Point(0, 0),
            ForeColor = Color.White,
            Text = "<No file loaded...>"
        };
        bytesTab.Controls.Add(bytesText);
        
        blocksView = new ListView(){
            Size = new Size(350, 350),
            Location = new Point(2, 2),
            View = View.Details,
            AllowColumnReorder = true,
            FullRowSelect = true,
            Activation = ItemActivation.OneClick,
            BackColor = Color.White,
            HideSelection = false
        };
        
        CreateBlocksViewColumnHeaders();
        blocksTab.Controls.Add(blocksView);

        createListBlock = new Button(){
            Size = new Size(100, 25),
            Location = new Point(2, 355),
            Text = "New block(s)...",
            BackColor = Color.White
        };
        blocksTab.Controls.Add(createListBlock);

        deleteBlock = new Button(){
            Size = new Size(100, 25),
            Location = new Point(250, 355),
            Text = "Delete block",
            BackColor = Color.White
        };
        blocksTab.Controls.Add(deleteBlock);

        listBlockUp = new Button(){
            Size = new Size(25, 25),
            Location = new Point(105, 355),
            Text = "\u2191",
            BackColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
        };
        blocksTab.Controls.Add(listBlockUp);

        listBlockDown = new Button(){
            Size = new Size(25, 25),
            Location = new Point(135, 355),
            Text = "\u2193",
            BackColor = Color.White,
            TextAlign = ContentAlignment.MiddleCenter
        };
        blocksTab.Controls.Add(listBlockDown);

        xyLabel = new Label(){
            Size = new Size(350, 200),
            Location = new Point(2, 395),
            ForeColor = Color.White,
            Text = "X:                                                   Y:\n\nHP:"
        };
        blocksTab.Controls.Add(xyLabel);

        xText = new TextBox()
        {
            Size = new Size(100, 25),
            Location = new Point(27, 392),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };
        blocksTab.Controls.Add(xText);
        xText.BringToFront();

        yText = new TextBox()
        {
            Size = new Size(100, 25),
            Location = new Point(191, 392),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };
        blocksTab.Controls.Add(yText);
        yText.BringToFront();

        hpText = new TextBox()
        {
            Size = new Size(300, 25),
            Location = new Point(27, 422),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };
        blocksTab.Controls.Add(hpText);
        hpText.BringToFront();

        ballFieldLabel = new Label(){
            Size = new Size(350, 200),
            Location = new Point(2, 43),
            ForeColor = Color.White,
            Text = "Horizontal Position:\n\nVertical Position:\n\nHorizontal Speed: \n\nVertical Speed:"
        };
        ballTab.Controls.Add(ballFieldLabel);

        ballxPosText = new TextBox()
        {
            Size = new Size(200, 25),
            Location = new Point(120, 40),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };
        ballTab.Controls.Add(ballxPosText);
        ballxPosText.BringToFront();

        ballyPosText = new TextBox()
        {
            Size = new Size(200, 25),
            Location = new Point(120, 70),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };
        ballTab.Controls.Add(ballyPosText);
        ballyPosText.BringToFront();

        ballxSpeedText = new TextBox()
        {
            Size = new Size(200, 25),
            Location = new Point(120, 100),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };
        ballTab.Controls.Add(ballxSpeedText);
        ballxSpeedText.BringToFront();

        ballySpeedText = new TextBox()
        {
            Size = new Size(200, 25),
            Location = new Point(120, 130),
            ForeColor = Color.Black,
            BackColor = Color.White,
        };
        ballTab.Controls.Add(ballySpeedText);
        ballySpeedText.BringToFront();

        showBallCheckbox = new CheckBox()
        {
            Size = new Size(300, 25),
            Location = new Point(5, 9),
            Text = "Show Ball", 
            Checked = true, 
            ForeColor = Color.White
        };
        ballTab.Controls.Add(showBallCheckbox);
        showBallCheckbox.BringToFront();
    }

    public void CreateBlocksViewColumnHeaders()
    {
        blocksView.Columns.AddRange(new ColumnHeader[] { 
            new ColumnHeader{
                Text = "Type",
                TextAlign = HorizontalAlignment.Center,
                Width = -2
            },
            new ColumnHeader{
                Text = "X Position",
                TextAlign = HorizontalAlignment.Center,
                Width = -2
            },
            new ColumnHeader{
                Text = "Y Position",
                TextAlign = HorizontalAlignment.Center,
                Width = -2
            },
            new ColumnHeader{
                Text = "Health",
                TextAlign = HorizontalAlignment.Center,
                Width = -2
            }
        });
    }

    #endregion

    private System.Windows.Forms.ComboBox levelSelect;
    private PictureBoxWithExtraSettings gameBackground;
    private PictureBoxWithExtraSettings ballImage;
    private System.Windows.Forms.Button saveToFile;
    private System.Windows.Forms.Button loadFromFile;
    private System.Windows.Forms.TabControl propertiesTab;
    private System.Windows.Forms.TabPage infoTab;
    private System.Windows.Forms.TabPage ballTab;
    private System.Windows.Forms.TabPage blocksTab;
    private System.Windows.Forms.TabPage bytesTab;
    private System.Windows.Forms.Label ballFieldLabel;
    private System.Windows.Forms.CheckBox showBallCheckbox;
    private System.Windows.Forms.TextBox ballxPosText;
    private System.Windows.Forms.TextBox ballyPosText;
    private System.Windows.Forms.TextBox ballxSpeedText;
    private System.Windows.Forms.TextBox ballySpeedText;
    private System.Windows.Forms.Button addLevelButton;
    private System.Windows.Forms.Button removeLevelButton;
    private System.Windows.Forms.Label bytesText;
    private System.Windows.Forms.ListView blocksView;
    private System.Windows.Forms.Button createListBlock;
    private System.Windows.Forms.Button deleteBlock;
    private System.Windows.Forms.Button listBlockUp;
    private System.Windows.Forms.Button listBlockDown;
    private System.Windows.Forms.Label xyLabel;
    private System.Windows.Forms.TextBox xText;
    private System.Windows.Forms.TextBox yText;
    private System.Windows.Forms.TextBox hpText;

}

/// <summary>
/// Inherits from PictureBox; adds more settings
/// </summary>
public class PictureBoxWithExtraSettings : PictureBox
{
    public InterpolationMode InterpolationMode { get; set; }
    public PixelOffsetMode PixelOffsetMode { get; set; }

    protected override void OnPaint(PaintEventArgs paintEventArgs)
    {
        paintEventArgs.Graphics.PixelOffsetMode = PixelOffsetMode;
        paintEventArgs.Graphics.InterpolationMode = InterpolationMode;
        base.OnPaint(paintEventArgs);
    }
}