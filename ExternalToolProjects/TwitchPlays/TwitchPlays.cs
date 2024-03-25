using System;
using System.Threading;
using BizHawk.Client.Common;
using BizHawk.Client.EmuHawk;
using TwitchPlays;

namespace TwitchPlays;

[ExternalTool("TwitchPlays")] // this appears in the Tools > External Tools submenu in EmuHawk
public sealed partial class TwitchPlays : ToolFormBase, IExternalToolForm
{
    protected override string WindowTitleStatic // required when superclass is ToolFormBase
        => "Twitch Plays setup";

    public TwitchPlays()
    {
        config = ConfigService.Load<TwitchPlaysConfig>(TwitchPlaysConfig.ControlDefaultPath);
        InitializeComponent();
    }

    public bool IsStarted;

    public ApiContainer? MaybeApiContainer { get; set; }

    private ApiContainer APIs => MaybeApiContainer!;

    public readonly TwitchPlaysConfig config;
    private Thread _ircThread;
    private SaveService _saveService;
    
    //UI component
    private System.Windows.Forms.Button tppLoadStateComponent;
    private System.Windows.Forms.Label label3;
    private System.Windows.Forms.TextBox tppChannelTwitchComponent;
    private System.Windows.Forms.Button tppSaveStateComponent;
    private System.Windows.Forms.Label label2;
    private System.Windows.Forms.TextBox tppOauthTokenComponent;
    private System.Windows.Forms.Button tppLaunchComponent;
    private System.Windows.Forms.TextBox tppLoginComponent;
    private System.Windows.Forms.Label label1;


    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
        this.label1 = new System.Windows.Forms.Label();
        this.tppLoginComponent = new System.Windows.Forms.TextBox();
        this.label2 = new System.Windows.Forms.Label();
        this.tppOauthTokenComponent = new System.Windows.Forms.TextBox();
        this.tppLaunchComponent = new System.Windows.Forms.Button();
        this.tppSaveStateComponent = new System.Windows.Forms.Button();
        this.label3 = new System.Windows.Forms.Label();
        this.tppChannelTwitchComponent = new System.Windows.Forms.TextBox();
        this.tppLoadStateComponent = new System.Windows.Forms.Button();
        this.SuspendLayout();
        // 
        // label1
        // 
        this.label1.Location = new System.Drawing.Point(12, 23);
        this.label1.Name = "label1";
        this.label1.Size = new System.Drawing.Size(100, 23);
        this.label1.TabIndex = 0;
        this.label1.Text = "Twitch login";
        // 
        // tppLoginComponent
        // 
        this.tppLoginComponent.Anchor =
            ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top |
                                                    System.Windows.Forms.AnchorStyles.Bottom) |
                                                   System.Windows.Forms.AnchorStyles.Left) |
                                                  System.Windows.Forms.AnchorStyles.Right)));
        this.tppLoginComponent.Location = new System.Drawing.Point(118, 20);
        this.tppLoginComponent.Name = "tppLoginComponent";
        this.tppLoginComponent.Size = new System.Drawing.Size(495, 26);
        this.tppLoginComponent.TabIndex = 1;
        this.tppLoginComponent.TextChanged += new System.EventHandler(this.LoginTextChanged);
        // 
        // label2
        // 
        this.label2.Location = new System.Drawing.Point(12, 58);
        this.label2.Name = "label2";
        this.label2.Size = new System.Drawing.Size(116, 23);
        this.label2.TabIndex = 2;
        this.label2.Text = "Oauth Token";
        // 
        // tppOauthTokenComponent
        // 
        this.tppOauthTokenComponent.Anchor =
            ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top |
                                                    System.Windows.Forms.AnchorStyles.Bottom) |
                                                   System.Windows.Forms.AnchorStyles.Left) |
                                                  System.Windows.Forms.AnchorStyles.Right)));
        this.tppOauthTokenComponent.Location = new System.Drawing.Point(118, 55);
        this.tppOauthTokenComponent.Name = "tppOauthTokenComponent";
        this.tppOauthTokenComponent.Size = new System.Drawing.Size(495, 26);
        this.tppOauthTokenComponent.TabIndex = 3;
        this.tppOauthTokenComponent.UseSystemPasswordChar = true;
        this.tppOauthTokenComponent.TextChanged += new System.EventHandler(this.OauthTextChanged);
        // 
        // tppLaunchComponent
        // 
        this.tppLaunchComponent.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.tppLaunchComponent.Location = new System.Drawing.Point(12, 139);
        this.tppLaunchComponent.Name = "tppLaunchComponent";
        this.tppLaunchComponent.Size = new System.Drawing.Size(108, 36);
        this.tppLaunchComponent.TabIndex = 4;
        this.tppLaunchComponent.Text = "Start";
        this.tppLaunchComponent.UseVisualStyleBackColor = true;
        this.tppLaunchComponent.Click += new System.EventHandler(this.OnStartStopClick);
        // 
        // tppSaveStateComponent
        // 
        this.tppSaveStateComponent.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.tppSaveStateComponent.Location = new System.Drawing.Point(451, 139);
        this.tppSaveStateComponent.Name = "tppSaveStateComponent";
        this.tppSaveStateComponent.Size = new System.Drawing.Size(162, 36);
        this.tppSaveStateComponent.TabIndex = 6;
        this.tppSaveStateComponent.Text = "Load save";
        this.tppSaveStateComponent.UseVisualStyleBackColor = true;
        this.tppSaveStateComponent.Click += new System.EventHandler(this.OnLoadSaveStateButtonClick);
        this.tppSaveStateComponent.Enabled = false;
        // 
        // label3
        // 
        this.label3.Location = new System.Drawing.Point(12, 98);
        this.label3.Name = "label3";
        this.label3.Size = new System.Drawing.Size(116, 23);
        this.label3.TabIndex = 7;
        this.label3.Text = "Twitch channel";
        // 
        // tppChannelTwitchComponent
        // 
        this.tppChannelTwitchComponent.Anchor =
            ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top |
                                                    System.Windows.Forms.AnchorStyles.Bottom) |
                                                   System.Windows.Forms.AnchorStyles.Left) |
                                                  System.Windows.Forms.AnchorStyles.Right)));
        this.tppChannelTwitchComponent.Location = new System.Drawing.Point(118, 95);
        this.tppChannelTwitchComponent.Name = "tppChannelTwitchComponent";
        this.tppChannelTwitchComponent.Size = new System.Drawing.Size(495, 26);
        this.tppChannelTwitchComponent.TabIndex = 8;
        this.tppChannelTwitchComponent.TextChanged += new System.EventHandler(this.ChannelTextChanged);

        // 
        // button1
        // 
        this.tppLoadStateComponent.Anchor = System.Windows.Forms.AnchorStyles.None;
        this.tppLoadStateComponent.Location = new System.Drawing.Point(283, 139);
        this.tppLoadStateComponent.Name = "tppLoadStateComponent";
        this.tppLoadStateComponent.Size = new System.Drawing.Size(162, 36);
        this.tppLoadStateComponent.TabIndex = 9;
        this.tppLoadStateComponent.Text = "Save";
        this.tppLoadStateComponent.UseVisualStyleBackColor = true;
        this.tppLoadStateComponent.Click += new System.EventHandler(OnSaveStateButtonClick);
        this.tppLoadStateComponent.Enabled = false;
        // 
        // BizHawkTpp
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.AutoSize = true;
        this.ClientSize = new System.Drawing.Size(635, 397);
        this.Controls.Add(this.tppLoadStateComponent);
        this.Controls.Add(this.tppChannelTwitchComponent);
        this.Controls.Add(this.label3);
        this.Controls.Add(this.tppSaveStateComponent);
        this.Controls.Add(this.tppLaunchComponent);
        this.Controls.Add(this.tppOauthTokenComponent);
        this.Controls.Add(this.label2);
        this.Controls.Add(this.tppLoginComponent);
        this.Controls.Add(this.label1);
        this.MaximumSize = new System.Drawing.Size(657, 453);
        this.MinimumSize = new System.Drawing.Size(657, 453);
        this.Name = "TwitchPlays";
        this.ResumeLayout(false);
        this.PerformLayout();
    }


    private void LoginTextChanged(object sender, EventArgs e)
    {
        config.Login = tppLoginComponent.Text;
    }

    private void OauthTextChanged(object sender, EventArgs e)
    {
        config.OauthToken = tppOauthTokenComponent.Text;
    }

    private void ChannelTextChanged(object sender, EventArgs e)
    {
        config.Channel = tppChannelTwitchComponent.Text;
    }

    private void OnStartStopClick(object sender, EventArgs e)
    {
        if (!IsStarted)
        {
            ConfigService.Save(TwitchPlaysConfig.ControlDefaultPath, config);
            var ircHandler = new IRCHandler(tppLoginComponent.Text, tppOauthTokenComponent.Text,
                tppChannelTwitchComponent.Text, APIs, _saveService, this);
            Thread thread = new Thread(ircHandler.RunIRC);
            _ircThread = thread;
            thread.Start();
            IsStarted = true;
            tppLaunchComponent.Text = "Stop";
            tppSaveStateComponent.Enabled = true;
            tppLoadStateComponent.Enabled = true;

        }
        else
        {
            _saveService.StopTask();
            _ircThread.Abort();
            IsStarted = false;
            tppLaunchComponent.Text = "Start";
            tppSaveStateComponent.Enabled = false;
            tppLoadStateComponent.Enabled = false;
        }
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);

        //Load config for tpp
        tppOauthTokenComponent.Text = config.OauthToken;
        tppLoginComponent.Text = config.Login;
        tppChannelTwitchComponent.Text = config.Channel;

        _saveService = new SaveService(APIs, this);
    }
    
    //TODO: CLose on nonerror : savestate.
    
    private void OnLoadSaveStateButtonClick(object sender, EventArgs e)
    {
        _saveService.Load();
    }
    
    private void OnSaveStateButtonClick(object sender, EventArgs e)
    {
        _saveService.Save();
    }
}