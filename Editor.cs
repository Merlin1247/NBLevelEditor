using System.Drawing.Drawing2D;
using System.Text;

#pragma warning disable CS8602 // Dereference of a possibly null reference. (Thrown when attempting to access a variable that may or may not be null)
#pragma warning disable CS8604 // Possible null reference argument. (Thrown when passing a variable that may or may not be null as a method parameter)

namespace BreakoutNESLevelEditor;

public partial class Editor : Form
{
    private int totalLevels;
    private List<LevelData> levelDataList = [];
    private byte[]? rawData;
    private string filePath = "";
    private List<BlockObj?> blockList = [];

    public Editor()
    {
        InitializeComponent();
        loadFromFile.Click += LoadDataFromFile;
        saveToFile.Click += SaveDataToFile;
        deleteLevel.Click += DeleteLevel;
        levelSelect.SelectedIndex = 0;
        levelSelect.SelectedIndexChanged += ChangeLevel;
        createListBlock.Click += ShowCreationDialog;
        deleteBlock.Click += DestroyBlock;
        blocksView.ItemSelectionChanged += SelectItem;
        listBlockUp.Click += ChangeBlockOrderUp;
        listBlockDown.Click += ChangeBlockOrderDown;
        filePathText.Leave += UpdateFilePath;
        showBallCheckbox.CheckedChanged += ToggleBallVisibility;
        ballxPosText.Leave += ChangeBallXPos;
        ballyPosText.Leave += ChangeBallYPos;
        ballxSpeedText.Leave += ChangeBallXSpd;
        ballySpeedText.Leave += ChangeBallYSpd;
        xText.Leave += ChangeX;
        yText.Leave += ChangeY;
        hpText.Leave += ChangeHP;
        this.KeyPreview = true;
        this.KeyDown += new KeyEventHandler(FormKeyDown);
        SetBlockOptionsVisibility(false);

        filePath = "LevelData.defaultlvl";
        LoadDataFromFile(null, null);
        filePath = "LevelData.lvl";
        filePathText.Text = filePath;
    }

    void FormKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Control && e.KeyCode == Keys.S)
        {
            SaveDataToFile(null, null);
            e.SuppressKeyPress = true;  
        }
        if (e.KeyData == Keys.Return || e.KeyData == Keys.Escape)
        {
            this.ActiveControl = null;
            e.SuppressKeyPress = true;  
        }
        if (e.Control && e.KeyCode == Keys.N && false)
        {
            StringStruct str = new StringStruct() { Value = filePath };
            filePath = "LevelData.defaultlvl";
            LoadDataFromFile(null, null);
            filePath = str.Value;
            e.SuppressKeyPress = true;  
        }
    }

    private void ShowCreationDialog(object? sender, EventArgs? e)
    {
        SelectBlockType sbt = new SelectBlockType();
        sbt.GetEditor(this);
        sbt.ShowDialog();
    }

    public void SelectItem(object? sender, EventArgs? e)
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        if(blocksView.SelectedItems.Count == 0)
        {
            SetBlockOptionsVisibility(false);
        }
        else if(blocksView.SelectedItems.Count == 1)
        {
            SetBlockOptionsVisibility(true);
            SetTangibilityAndColour(listBlockUp, blocksView.SelectedItems[0].Index != 0);
            SetTangibilityAndColour(listBlockDown, blocksView.SelectedItems[0].Index != blocksView.Items.Count - 1);
            yText.Text = currentLevel.blocks[blocksView.SelectedItems[0].Index].yPos.ToString();
            hpText.Text = currentLevel.blocks[blocksView.SelectedItems[0].Index].health[0].ToString();
            SetTangibilityAndColour(xText, currentLevel.blocks[blocksView.SelectedItems[0].Index].type == 0);
            if(xText.Enabled)
            {
                xText.Text = currentLevel.blocks[blocksView.SelectedItems[0].Index].xPos.ToString();
            }
            else
            {
                xText.Text = "-";
                for(int i = 1; i < 14; i++)
                { hpText.Text += "," + currentLevel.blocks[blocksView.SelectedItems[0].Index].health[i].ToString();}
            }
        }
        else if(blocksView.SelectedItems.Count > 1)
        {
            SetBlockOptionsVisibility(true);
            SetTangibilityAndColour(xText, true);
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
        listBlockUp.Visible = visible;
        listBlockDown.Visible = visible;
    }

    public static void SetTangibilityAndColour(dynamic obj, bool enabled)
    {
        if(obj == null) { return; }
        obj.Enabled = enabled;
        if(enabled) { obj.BackColor = Color.White; }
        else { obj.BackColor = Color.LightGray; }
    }

    public void FinishBlockCreation(byte mode)
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        switch(mode)
        {
            case 0:
                if(currentLevel.blocks != null)
                {
                    currentLevel.blocks.Add(
                        new Block{
                            xPos = 1,
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
        UpdateDisplayedLevel();
    }

    public void UpdateDisplayedLevel(bool noBlocks = false)
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        if(noBlocks) { goto BLOCKEND; }

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
                        { l.SubItems[3].Text += "," + s.health[i]; 
                        CreateBlock((byte)(i + 1), s.yPos, s.health[i]); }
                        break;
                    default:
                        throw new FormatException();
                }
                blocksView.Items.Add(l);
            }
        }

        BLOCKEND:

        ballImage.BringToFront();
        UpdateBallFields();

        currentLevel.data = [
            (byte)(currentLevel.ballData.xSpeed & 0b11111111),
            (byte)(currentLevel.ballData.ySpeed & 0b11111111),
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

        int tb = 0;
        foreach(LevelData l in levelDataList)
        { tb += l.data.Count; }
        infoLabel.Text = $"File Save Path:\n\n\n\nByte count (current level):   {currentLevel.data.Count}/256\nByte count (all levels):          {tb}";
        
        bytesText.Text = "";
        foreach(LevelData l in levelDataList)
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
        if(levelSelect.SelectedIndex >= levelDataList.Count)
        {
            levelDataList.Add(new LevelData(){
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
            SetTangibilityAndColour(deleteLevel, true);
        }
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        ballImage.Location = new Point(currentLevel.ballData.xPos * 2 + 8, currentLevel.ballData.yPos * 2 + 36);
        UpdateBallFields();
        
        UpdateDisplayedLevel();
    }

    private void DeleteLevel(object? sender, EventArgs? e)
    {
        if(MessageBox.Show("Are you sure you want to delete this level?", "Confirm action", MessageBoxButtons.OKCancel) == DialogResult.OK)
        {
            levelDataList.RemoveAt(levelSelect.SelectedIndex);
            int si = Math.Max(0, levelSelect.SelectedIndex - 1);
            levelSelect.Items.Clear();
            for(int i = 0; i < levelDataList.Count; i++) { levelSelect.Items.Add("Level " + (i + 1)); }
            levelSelect.Items.Add("New Level...");
            levelSelect.SelectedIndex = si;
            SetTangibilityAndColour(deleteLevel, levelDataList.Count > 1);
        }
    }

    private void CreateBlock(byte x, byte y, byte hp)
    {
        if(hp < 0 || hp > 100 || x < 1 || x > 14 || y < 2 || y > 12) {throw new ArgumentOutOfRangeException(); }
        if(hp == 0) return;
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
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        if(currentLevel.blocks != null)
        {
            for(int i = 0; i < blocksView.SelectedItems.Count; i++)
            {
                currentLevel.blocks.RemoveAt(blocksView.SelectedItems[0].Index);
            }
        }
        UpdateDisplayedLevel();
        SetBlockOptionsVisibility(false);
    }

    public void ChangeBlockOrderUp(object? sender, EventArgs? e)
    { if(blocksView.SelectedItems[0].Index == 0) { throw new ArgumentOutOfRangeException();} 
    ChangeBlockOrder(-1); }
    public void ChangeBlockOrderDown(object? sender, EventArgs? e)
    { if(blocksView.SelectedItems[0].Index + 1 == blocksView.Items.Count) { throw new ArgumentOutOfRangeException();} 
    ChangeBlockOrder(1); }
    public void ChangeBlockOrder(int offset)
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        Block b = currentLevel.blocks[blocksView.SelectedItems[0].Index];
        currentLevel.blocks.RemoveAt(blocksView.SelectedItems[0].Index);
        currentLevel.blocks.Insert(blocksView.SelectedItems[0].Index + offset, b);
        blocksView.Items[blocksView.SelectedItems[0].Index + offset].Selected = true;
        blocksView.Items[blocksView.SelectedItems[0].Index - offset].Selected = false;
        blocksView.Select();
        UpdateAndPreserveSelection();
    }

    private void ChangeX(object? sender, EventArgs? e)
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
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
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
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
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        if(currentLevel.blocks != null)
        {
            try
            {
                for(int i = 0; i < blocksView.SelectedItems.Count; i++)
                {
                    switch(currentLevel.blocks[blocksView.SelectedItems[i].Index].type)
                    {
                        case 0:
                            byte b0 = Convert.ToByte(hpText.Text);
                            if(b0 == 0 || b0 > 100) throw new ArgumentOutOfRangeException();
                            currentLevel.blocks[blocksView.SelectedItems[i].Index].health[0] = b0;
                            break;
                        case 1:
                            string[] str = hpText.Text.Split(",");
                            for(int j = 0; j < 14; j++)
                            {
                                byte b1 = Convert.ToByte(str[j]);
                                if(b1 > 100) throw new ArgumentOutOfRangeException();
                                currentLevel.blocks[blocksView.SelectedItems[i].Index].health[j] = b1;
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
        UpdateDisplayedLevel();
        foreach(int n1 in n) { blocksView.Items[n1].Selected = true; }
        blocksView.Select();
    }

    private void ToggleBallVisibility(object? sender, EventArgs e)
    {
        ballImage.Visible = showBallCheckbox.Checked;
    }

    private void ChangeBallXPos(object? sender, EventArgs e)
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        try { currentLevel.ballData.xPos = Convert.ToByte(Math.Clamp(Convert.ToSingle(ballxPosText.Text), 0x10, 256-0x15)); }
        catch { ballxPosText.Text = currentLevel.ballData.xPos.ToString(); return;}
        ballImage.Location = new Point(currentLevel.ballData.xPos * 2 + 8, currentLevel.ballData.yPos * 2 + 36);
        currentLevel.data[4] = currentLevel.ballData.xPos;
        UpdateDisplayedLevel(true);
    }

    private void ChangeBallYPos(object? sender, EventArgs e)
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        try { currentLevel.ballData.yPos = Convert.ToByte(Math.Clamp(Convert.ToSingle(ballyPosText.Text), 0x18, 256-0x25)); }
        catch { ballyPosText.Text = currentLevel.ballData.yPos.ToString(); return;}
        ballImage.Location = new Point(currentLevel.ballData.xPos * 2 + 8, currentLevel.ballData.yPos * 2 + 36);
        currentLevel.data[5] = currentLevel.ballData.yPos;
        UpdateDisplayedLevel(true);
    }

    

    private void ChangeBallXSpd(object? sender, EventArgs e)
    {
        StringStruct s = new(){ Value = ballxSpeedText.Text };
        FloatStruct f = new();
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        try { f.Value = Math.Clamp(Convert.ToSingle(s.Value), -4, 4); }
        catch { UpdateBallFields(); return;}
        currentLevel.ballData.xSpeed = Convert.ToSByte(Math.Floor(f.Value));
        if(Math.Round((Math.Clamp(Convert.ToSingle(f.Value), -4, 4) - currentLevel.ballData.xSpeed) * 256) == 256) 
        { currentLevel.ballData.xSpeed = Convert.ToSByte(f.Value); currentLevel.ballData.xSubSpeed = 0; }
        else { currentLevel.ballData.xSubSpeed = Convert.ToByte(Math.Round((f.Value - currentLevel.ballData.xSpeed) * 256)); }
        UpdateDisplayedLevel(true);
    }

    private void ChangeBallYSpd(object? sender, EventArgs e)
    {
        StringStruct s = new(){ Value = ballySpeedText.Text };
        FloatStruct f = new();
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        try { f.Value = Math.Clamp(Convert.ToSingle(s.Value), -4, 4); }
        catch { UpdateBallFields(); return;}
        currentLevel.ballData.ySpeed = Convert.ToSByte(Math.Floor(f.Value));
        if(Math.Round((Math.Clamp(Convert.ToSingle(f.Value), -4, 4) - currentLevel.ballData.ySpeed) * 256) == 256) 
        { currentLevel.ballData.ySpeed = Convert.ToSByte(f.Value); currentLevel.ballData.ySubSpeed = 0; }
        else { currentLevel.ballData.ySubSpeed = Convert.ToByte(Math.Round((f.Value - currentLevel.ballData.ySpeed) * 256)); }
        UpdateDisplayedLevel(true);
    }

    public void UpdateBallFields()
    {
        LevelData currentLevel = levelDataList[levelSelect.SelectedIndex];
        ballxPosText.Lines = [currentLevel.ballData.xPos + ""];
        ballyPosText.Lines = [currentLevel.ballData.yPos + ""];
        ballxSpeedText.Lines = [(Convert.ToSingle(currentLevel.ballData.xSubSpeed) / 256 + currentLevel.ballData.xSpeed).ToString()];
        ballySpeedText.Lines = [(Convert.ToSingle(currentLevel.ballData.ySubSpeed) / 256 + currentLevel.ballData.ySpeed).ToString()];
    }

    private void UpdateFilePath(object? sender, EventArgs? e)
    { filePath = filePathText.Text; }

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
                filePathText.Text = filePath;
            }
            else { return; }            
        }
        READFILE:
        
        levelDataList = [];
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
            LevelData currentLevel = new(){
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
                        levelDataList.Add(currentLevel);
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
            levelDataList.Add(currentLevel);
            totalLevels++;
            globalOffset += levelOffset;
        }
        FINISHDATA:
        totalLevels++;
        levelSelect.Items.Clear();
        SetTangibilityAndColour(deleteLevel, totalLevels > 1);
        for(int i = 0; i < totalLevels; i++) { levelSelect.Items.Add("Level " + (i + 1)); }
        levelSelect.Items.Add("New Level...");
        levelSelect.SelectedIndex = 0;
        //ChangeLevel(null, null);
    }

    public void SaveDataToFile(object? sender, EventArgs? e)
    {
        if(levelDataList.Count > 256)
        { MessageBox.Show($"There are more than 256 levels ({levelDataList.Count})", "Failed to save level data"); return; }
        for(int i = 0; i < levelDataList.Count; i++)
        {
            if(levelDataList[i].data.Count > 256) 
            { MessageBox.Show($"Level {i + 1} contains more than 256 bytes of data ({levelDataList[i].data.Count})", "Failed to save level data"); return; }
        }

        List<byte> b = [];
        for(int i = 0; i < levelDataList.Count; i++)
        {
            b.AddRange(levelDataList[i].data);
        }

        if(b.Count >= 25000)
        { MessageBox.Show("The level data is too big (>25kb)", "Failed to save level data"); return; }

        rawData = [.. b];
        FileStream fs = new(filePath, FileMode.Create);
        fs.Write(rawData);
        fs.Close();

        string inc = ".define LEVELDATAINDICES LD";
        int offset = levelDataList[0].data.Count;
        for(int i = 1; i < levelDataList.Count; i++)
        {
            inc += ", LD+" + offset;
            offset += levelDataList[i].data.Count;
        }
        fs = new(filePath.Remove(filePath.Length - 3) + "offsets.inc.s", FileMode.Create);
        fs.Write(Encoding.UTF8.GetBytes(inc));
        fs.Close();

        if(b.Count >= 12000)
        { MessageBox.Show("Friendly reminder that the level data is beginning to grow large.\n(Note: The level data has been saved successfully)", "Filesize warning"); }
    }
}

struct StringStruct
{
    public string Value { get; set; }
}

struct FloatStruct
{
    public float Value { get; set; }
}

public class LevelData
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