using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms.VisualStyles;

#pragma warning disable CS8602 // Dereference of a possibly null reference. (Thrown when attempting to access a variable that may or may not be null)

namespace BreakoutNESLevelEditor;

public partial class Editor : Form
{
    private int totalLevels;
    private List<LevelDatum> levelData = [];
    private byte[]? rawData;
    private string filePath = "";
    private List<BlockObj?> blockList = [];

    public Editor()
    {
        InitializeComponent();
        loadFromFile.Click += LoadDataFromFile;
        saveToFile.Click += SaveDataToFile;
        levelSelect.SelectedIndex = 0;
        levelSelect.SelectedIndexChanged += ChangeLevel;
        createListBlock.Click += ShowCreationDialog;
        deleteBlock.Click += DestroyBlock;
        blocksView.ItemSelectionChanged += SelectItem;
        listBlockUp.Click += ChangeBlockOrderUp;
        listBlockDown.Click += ChangeBlockOrderDown;
        showBallCheckbox.CheckedChanged += ToggleBallVisibility;
        ballxPosText.Leave += ChangeBallXPos;
        ballyPosText.Leave += ChangeBallYPos;
        ballxSpeedText.Leave += ChangeBallXSpd;
        ballySpeedText.Leave += ChangeBallYSpd;
        xText.Leave += ChangeX;
        yText.Leave += ChangeY;
        hpText.Leave += ChangeHP;
        SetBlockOptionsVisibility(false);

        filePath = "LevelData.defaultlvl";
        LoadDataFromFile(null, null);
        filePath = "LevelData.lvl";
    }

    private void ShowCreationDialog(object? sender, EventArgs? e)
    {
        SelectBlockType sbt = new SelectBlockType();
        sbt.GetEditor(this);
        sbt.ShowDialog();
    }

    public void SelectItem(object? sender, EventArgs? e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        if(blocksView.SelectedItems.Count == 0)
        {
            SetBlockOptionsVisibility(false);
        }
        else if(blocksView.SelectedItems.Count == 1)
        {
            SetBlockOptionsVisibility(true);
            yText.Text = currentLevel.blocks[blocksView.SelectedItems[0].Index].yPos.ToString();
            hpText.Text = currentLevel.blocks[blocksView.SelectedItems[0].Index].health[0].ToString();
            xText.Enabled = currentLevel.blocks[blocksView.SelectedItems[0].Index].type == 0;
            if(xText.Enabled)
            {
                xText.Text = currentLevel.blocks[blocksView.SelectedItems[0].Index].xPos.ToString();
            }
            else
            {
                xText.Text = "-";
                for(int i = 1; i < 14; i++)
                { hpText.Text += ", " + currentLevel.blocks[blocksView.SelectedItems[0].Index].health[i].ToString();}
            }
        }
        else if(blocksView.SelectedItems.Count > 1)
        {
            SetBlockOptionsVisibility(true);
            xText.Enabled = true;
            xText.Text = "-";
            yText.Text = "-";
            hpText.Text = "-";
        }
    }

    public void SetBlockOptionsVisibility(bool visible)
    {
        deleteBlock.Visible = visible;
        xyLabel.Visible = visible;
        xText.Visible = visible;
        yText.Visible = visible;
        hpText.Visible = visible;
    }

    public void FinishBlockCreation(byte mode)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        switch(mode)
        {
            case 0:
                if(currentLevel.blocks != null)
                {
                    currentLevel.blocks.Add(
                        new Block{
                            xPos = 2,
                            yPos = 2,
                            health = [1]
                        }
                    );
                }
                break;
            case 1:
                if(currentLevel.blocks != null)
                {
                    currentLevel.blocks.Add(
                        new Block{
                            yPos = 2,
                            type = 1,
                            health = [1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1]
                        }
                    );
                }
                break;
            case 2:
                //break;
            default:
                throw new ArgumentOutOfRangeException();
        }
        UpdateBlocks();
    }

    public void UpdateBlocks()
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        if(blockList != null)
        {
            foreach(BlockObj? b in blockList)
            {
                b.box.Controls.Clear();
                this.Controls.Remove(b.box);
            }
            blockList.Clear();
        }
        blocksView.Items.Clear();
        //CreateBlocksViewColumnHeaders();
        if(currentLevel.blocks != null)
        {
            foreach(Block s in currentLevel.blocks)
            {
                ListViewItem l = new ListViewItem();
                l.SubItems.AddRange(new ListViewItem.ListViewSubItem[] {
                    new ListViewItem.ListViewSubItem(l, s.xPos.ToString()),
                    new ListViewItem.ListViewSubItem(l, s.yPos.ToString()),
                    new ListViewItem.ListViewSubItem(l, s.health[0].ToString())
                });
                switch(s.type)
                {
                    case 0:
                        l.Text = "Simple";
                        CreateBlock(s.xPos, s.yPos, s.health[0]);
                        break;
                    case 1:
                        l.Text = "Line";
                        CreateBlock(1, s.yPos, s.health[0]);
                        l.SubItems[1].Text = "-";
                        for(int i = 1; i < 14; i++)
                        { l.SubItems[3].Text += ", " + s.health[i]; 
                        CreateBlock((byte)(i + 1), s.yPos, s.health[i]); }
                        break;
                    default:
                        throw new FormatException();
                }
                blocksView.Items.Add(l);
            }
        }

        ballImage.BringToFront();

        currentLevel.data = [
            (byte)(currentLevel.ballData.xSpeed & 0b10000011),
            (byte)(currentLevel.ballData.ySpeed & 0b10000011),
            currentLevel.ballData.xSubSpeed,
            currentLevel.ballData.ySubSpeed,
            currentLevel.ballData.xPos,
            currentLevel.ballData.yPos
        ];
        foreach (Block b in currentLevel.blocks)
        {
            switch(b.type)
            {
                case 0:
                    currentLevel.data.AddRange([
                        (byte)(b.health[0] & 0b01111111), 
                        (byte)( b.yPos << 4 | b.xPos & 0b00001111)
                    ]);
                    break;
                case 1:
                    currentLevel.data.Add(
                    (byte)((byte)((b.yPos & 0b00000011) << 5 | (b.yPos & 0b00001100) >> 2 | ((b.type - 1) & 0b00000111) << 2) | 0b10000000));
                    for(int i = 0; i < 14; i++)
                    { currentLevel.data.Add(b.health[i]); }
                    break;
                default:
                    throw new FormatException();
            }

        }
        currentLevel.data.Add(0);
        
        bytesText.Text = "";
        foreach(LevelDatum l in levelData)
        {
            foreach(byte b in l.data)
            {
                if(b < 0x10) { bytesText.Text += "0"; }
                bytesText.Text += b.ToString("X") + "  ";
                //if(b != rawData.Length - 1) { bytesText.Text += ", "; }
            }
        }
    }

    private void ChangeLevel(object? sender, EventArgs? e)
    {
        if(levelSelect.SelectedIndex >= levelData.Count)
        {
            levelData.Add(new LevelDatum(){
                header = 0,
                ballData = new BallData(){
                    xSpeed = 0,
                    ySpeed = 1,
                    xSubSpeed = 0,
                    ySubSpeed = 0,
                    xPos = 0x7E,
                    yPos = 0x7E
                },
                blocks = [],
                data = []
            });
            levelSelect.Items[levelSelect.SelectedIndex] = "Level " + (levelSelect.SelectedIndex + 1);
            levelSelect.Items.Add("New Level...");
        }
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        ballImage.Location = new Point(currentLevel.ballData.xPos * 2 + 8, currentLevel.ballData.yPos * 2 + 36);
        UpdateBallFields();
        
        UpdateBlocks();
    }

    private void CreateBlock(byte x, byte y, byte hp)
    {
        Console.WriteLine("creating... " + x + " " + y + " " + hp);
        if(hp < 1 || hp > 100 || x < 1 || x > 14 || y < 2 || y > 12) {throw new ArgumentOutOfRangeException(); }
        BlockObj b = new BlockObj(){
            health = hp,
            xPos = x,
            yPos = y,
            box = new PictureBoxWithExtraSettings(){
                Size = new Size(32, 32),
                Location = new Point(x * 32 + 8, y * 32 + 36),
                SizeMode = PictureBoxSizeMode.StretchImage,
                ImageLocation = @"Images\Blocks\whitebox.png",
                InterpolationMode = InterpolationMode.NearestNeighbor,
                PixelOffsetMode = PixelOffsetMode.Half
            }
        };
        if(hp != 100)
        {
            b.digitL = new PictureBoxWithExtraSettings(){
                Size = new Size(8, 14),
                Location = new Point(6, 10),
                SizeMode = PictureBoxSizeMode.StretchImage,
                ImageLocation = @"Images\BlockNumbers\" + Math.Floor((double)hp / 10) + ".png",
                InterpolationMode = InterpolationMode.NearestNeighbor,
                PixelOffsetMode = PixelOffsetMode.Half
            };
            b.digitR = new PictureBoxWithExtraSettings(){
                Size = new Size(8, 14),
                Location = new Point(16, 10),
                SizeMode = PictureBoxSizeMode.StretchImage,
                ImageLocation = @"Images\BlockNumbers\" + hp % 10 + ".png",
                InterpolationMode = InterpolationMode.NearestNeighbor,
                PixelOffsetMode = PixelOffsetMode.Half
            };
        }
        else
        {
            b.digitL = new PictureBoxWithExtraSettings(){
                Size = new Size(20, 14),
                Location = new Point(6, 10),
                SizeMode = PictureBoxSizeMode.StretchImage,
                ImageLocation = @"Images\BlockNumbers\100.png",
                InterpolationMode = InterpolationMode.NearestNeighbor,
                PixelOffsetMode = PixelOffsetMode.Half
            };
        }
        this.Controls.Add(b.box);
        b.box.BringToFront();
        b.box.Controls.Add(b.digitL);
        b.digitL.BringToFront();
        if(hp != 100)
        {
            b.box.Controls.Add(b.digitR);
            b.digitR.BringToFront();
        }
        blockList?.Add(b);
    }

    private void DestroyBlock(object? sender, EventArgs? e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        if(currentLevel.blocks != null)
        {
            for(int i = 0; i < blocksView.SelectedItems.Count; i++)
            {
                currentLevel.blocks.RemoveAt(blocksView.SelectedItems[0].Index);
            }
        }
        UpdateBlocks();
    }

    public void ChangeBlockOrderUp(object? sender, EventArgs? e)
    { if(blocksView.SelectedItems[0].Index == 0) { throw new ArgumentOutOfRangeException();} ChangeBlockOrder(-1); }
    public void ChangeBlockOrderDown(object? sender, EventArgs? e)
    { if(blocksView.SelectedItems[0].Index + 1 == blocksView.Items.Count) { throw new ArgumentOutOfRangeException();} 
    ChangeBlockOrder(1); }
    public void ChangeBlockOrder(int offset)
    {
        
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        Block b = currentLevel.blocks[blocksView.SelectedItems[0].Index];
        currentLevel.blocks.RemoveAt(blocksView.SelectedItems[0].Index);
        currentLevel.blocks.Insert(blocksView.SelectedItems[0].Index + offset, b);
        UpdateBlocks();
    }

    private void ChangeX(object? sender, EventArgs? e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        if(currentLevel.blocks != null)
        {
            try
            {
                for(int i = 0; i < blocksView.SelectedItems.Count; i++)
                {
                    currentLevel.blocks[blocksView.SelectedItems[i].Index].xPos = Convert.ToByte(xText.Text);
                }
                UpdateAndPreserveSelection();
            }
            catch
            {
                SelectItem(null, null);
            }
        }
    }
    private void ChangeY(object? sender, EventArgs? e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        if(currentLevel.blocks != null)
        {
            try
            {
                for(int i = 0; i < blocksView.SelectedItems.Count; i++)
                {
                    currentLevel.blocks[blocksView.SelectedItems[i].Index].yPos = Convert.ToByte(yText.Text);
                }
                UpdateAndPreserveSelection();
            }
            catch
            {
                SelectItem(null, null);
            }
        }
    }

    private void ChangeHP(object? sender, EventArgs? e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        if(currentLevel.blocks != null)
        {
            try
            {
                for(int i = 0; i < blocksView.SelectedItems.Count; i++)
                {
                    switch(currentLevel.blocks[blocksView.SelectedItems[i].Index].type)
                    {
                        case 0:
                            currentLevel.blocks[blocksView.SelectedItems[i].Index].health[0] = Convert.ToByte(hpText.Text);
                            break;
                        case 1:
                            string[] str = hpText.Text.Split(", ");
                            for(int j = 0; j < 14; j++)
                            {
                                currentLevel.blocks[blocksView.SelectedItems[i].Index].health[j] = Convert.ToByte(str[j]);
                            }
                            break;
                        case 2:
                        default:
                            throw new FormatException();
                    }
                    
                }
                UpdateAndPreserveSelection();
            }
            catch
            {
                SelectItem(null, null);
            }
        }
    }

    private void UpdateAndPreserveSelection()
    {
        List<int> n = [];
        foreach(ListViewItem l in blocksView.SelectedItems) { n.Add(l.Index); }
        UpdateBlocks();
        foreach(int n1 in n) { blocksView.Items[n1].Selected = true; }
        blocksView.Select();
    }

    private void ToggleBallVisibility(object? sender, EventArgs e)
    {
        ballImage.Visible = showBallCheckbox.Checked;
    }

    private void ChangeBallXPos(object? sender, EventArgs e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        currentLevel.ballData.xPos = Convert.ToByte(ballxPosText.Text);
        ballImage.Location = new Point(currentLevel.ballData.xPos * 2 + 8, currentLevel.ballData.yPos * 2 + 36);
        ballxPosText.Text = currentLevel.ballData.xPos.ToString();
        currentLevel.data[4] = currentLevel.ballData.xPos;
    }

    private void ChangeBallYPos(object? sender, EventArgs e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        currentLevel.ballData.yPos = Convert.ToByte(ballyPosText.Text);
        ballImage.Location = new Point(currentLevel.ballData.xPos * 2 + 8, currentLevel.ballData.yPos * 2 + 36);
        ballyPosText.Text = currentLevel.ballData.yPos.ToString();
        currentLevel.data[5] = currentLevel.ballData.yPos;
    }

    private void ChangeBallXSpd(object? sender, EventArgs e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        currentLevel.ballData.xSpeed = Convert.ToSByte(Math.Floor(Convert.ToSingle(ballxSpeedText.Text)));
        currentLevel.ballData.xSubSpeed = Convert.ToByte(Math.Round((Convert.ToSingle(ballxSpeedText.Text) - currentLevel.ballData.xSpeed) * 256));
        ballxSpeedText.Text = (currentLevel.ballData.xSubSpeed / 256 + currentLevel.ballData.xSpeed).ToString();
        currentLevel.data[0] = (byte)currentLevel.ballData.xSpeed;
        currentLevel.data[2] = currentLevel.ballData.xSubSpeed;
    }

    private void ChangeBallYSpd(object? sender, EventArgs e)
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        currentLevel.ballData.ySpeed = Convert.ToSByte(Math.Floor(Convert.ToSingle(ballySpeedText.Text)));
        currentLevel.ballData.ySubSpeed = Convert.ToByte(Math.Round((Convert.ToSingle(ballySpeedText.Text) - currentLevel.ballData.ySpeed) * 256));
        Console.WriteLine(Convert.ToByte(Math.Round((Convert.ToSingle(ballySpeedText.Text) - currentLevel.ballData.ySpeed) * 256)) + "");
        ballySpeedText.Text = (Convert.ToSingle(currentLevel.ballData.ySubSpeed) / 256 + currentLevel.ballData.ySpeed).ToString();
        currentLevel.data[1] = (byte)currentLevel.ballData.ySpeed;
        currentLevel.data[3] = currentLevel.ballData.ySubSpeed;
    }

    public void UpdateBallFields()
    {
        LevelDatum currentLevel = levelData[levelSelect.SelectedIndex];
        ballxPosText.Text = currentLevel.ballData.xPos.ToString();
        ballyPosText.Text = currentLevel.ballData.yPos.ToString();
        ballxSpeedText.Text = (currentLevel.ballData.xSubSpeed / 256 + currentLevel.ballData.xSpeed).ToString();
        ballySpeedText.Text = (currentLevel.ballData.ySubSpeed / 256 + currentLevel.ballData.ySpeed).ToString();
    }

    private void LoadDataFromFile(object? sender, EventArgs? e)
    {
        if(sender == null) { goto READFILE; }
        using (OpenFileDialog openFileDialog = new OpenFileDialog())
        {
            openFileDialog.InitialDirectory = "d:\\Apps\\BreakoutNES";
            openFileDialog.Filter = "lvl files (*.lvl)|*.lvl|All files (*.*)|*.*";
            openFileDialog.FilterIndex = 1;
            openFileDialog.RestoreDirectory = true;

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                //Get the path of specified file
                filePath = openFileDialog.FileName;
            }
            else { return; }            
        }
        READFILE:
        
        levelData = [];
        FileStream fs = new(filePath, FileMode.Open);
        BinaryReader bf = new(fs);
        rawData = bf.ReadBytes((int)bf.BaseStream.Length);
        fs.Close();

        bytesText.Text = "";
        foreach(byte b in rawData)
        {
            if(b < 0x10) { bytesText.Text += "0"; }
            bytesText.Text += b.ToString("X") + "  ";
            //if(b != rawData.Length - 1) { bytesText.Text += ", "; }
        }
        
        int globalOffset = 0;
        while(true)
        {
            byte levelOffset = 0;
            LevelDatum currentLevel = new(){
                header = (byte)(rawData[globalOffset] & 0b01111100),
                ballData = new BallData(){
                    xSpeed = (sbyte)(rawData[globalOffset] & 0b10000011),
                    ySpeed = (sbyte)(rawData[globalOffset+1] & 0b10000011),
                    xSubSpeed = rawData[globalOffset+2],
                    ySubSpeed = rawData[globalOffset+3],
                    xPos = rawData[globalOffset+4],
                    yPos = rawData[globalOffset+5]
                },
                blocks = [],
                data = []
            };
            levelOffset += 6;
            while(true)
            {
                if(rawData[globalOffset+levelOffset] == 0) 
                {
                    if(globalOffset + levelOffset + 1 == rawData.Length) 
                    { 
                        levelOffset++;
                        for(int i = 0; i < levelOffset; i++)
                        {
                            currentLevel.data.Add(rawData[globalOffset + i]);
                        }
                        levelData.Add(currentLevel);
                        goto FINISHDATA; 
                    }
                    goto FINISHLEVEL; 
                } 
                byte blockHeader = rawData[globalOffset+levelOffset];
                byte type = (byte)(blockHeader & 0b10000000);
                if(type == 0) //single block
                {
                    currentLevel.blocks.Add(new Block(){
                        health = [(byte)(blockHeader & 0b01111111)],
                        xPos = (byte)(rawData[globalOffset+levelOffset+1] & 0b00001111),
                        yPos = (byte)((rawData[globalOffset+levelOffset+1] & 0b11110000) >>> 4)
                    });
                    levelOffset += 2;
                }
                else //Line of blocks
                {
                    Block currentLine = new Block();
                    currentLine.yPos = (byte)(((blockHeader & 0b01100000) >> 5) | ((blockHeader & 0b00000011) << 2));
                    currentLine.type = (byte)(((blockHeader & 0b00011100) >> 2) + 1);
                    switch(currentLine.type)
                    {
                        case 1:
                            byte[] currentData = new byte[14];
                            for(int i = 0; i < 14; i++)
                            {
                                currentData[i] = rawData[globalOffset+levelOffset+i+1];
                            }
                            currentLine.health = currentData;
                            levelOffset += 15;
                            break;
                        case 2:
                            break;
                        case 3:
                            break;
                        case 4:
                            break;
                        default:
                            throw new FileFormatException();
                    }
                    currentLevel.blocks.Add(currentLine);
                }
            }
            FINISHLEVEL:
            levelOffset++;
            for(int i = 0; i < levelOffset; i++)
            {
                currentLevel.data.Add(rawData[globalOffset + i]);
            }
            levelData.Add(currentLevel);
            totalLevels++;
            globalOffset += levelOffset;
        }
        FINISHDATA:
        totalLevels++;
        levelSelect.Items.Clear();
        for(int i = 0; i < totalLevels; i++) { levelSelect.Items.Add("Level " + (i + 1)); }
        levelSelect.Items.Add("New Level...");
        levelSelect.SelectedIndex = 0;
        //ChangeLevel(null, null);
    }

    public void SaveDataToFile(object? sender, EventArgs? e)
    {
        if(File.Exists(filePath) == false) { return; }
        List<byte> b = [];
        foreach(LevelDatum l in levelData)
        {
            for(int i = 0; i < l.data.Count; i++)
            {
                b.Add(l.data[i]);
            }
        }
        rawData = [.. b];
        FileStream fs = new(filePath, FileMode.Create);
        fs.Write(rawData);
        fs.Close();

        string inc = ".define LEVELDATAINDICES LD";
        int offset = levelData[0].data.Count;
        for(int i = 1; i < levelData.Count; i++)
        {
            inc += ", LD+" + offset;
            offset += levelData[i].data.Count;
        }
        fs = new(filePath.Remove(filePath.Length - 3) + "offsets.inc.s", FileMode.Create);
        fs.Write(Encoding.UTF8.GetBytes(inc));
        fs.Close();
    }
}

public class LevelDatum
{
    public byte header;
    public BallData? ballData;
    public List<Block>? blocks;
    public List<byte>? data;
}

public class Block
{
    public byte[] health = [];
    public byte xPos;
    public byte yPos;
    public byte type;
}

public class BallData
{
    public sbyte xSpeed;
    public byte xSubSpeed;
    public byte xPos;
    public sbyte ySpeed;
    public byte ySubSpeed;
    public byte yPos;
}

public class BlockObj
{
    public byte health;
    public byte xPos;
    public byte yPos;
    public PictureBoxWithExtraSettings? box;
    public PictureBoxWithExtraSettings? digitL;
    public PictureBoxWithExtraSettings? digitR;
}