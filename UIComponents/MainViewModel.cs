using Cysharp.Diagnostics;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.IO;
using System.Reactive;
using System.Reactive.Subjects;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;

namespace FMOD_DecompilerUI.ViewModels;

public class MainViewModel : ViewModelBase
{
    public Interaction<string, string> PickAFolder { get; } = new();

    public Interaction<string[], Unit> MsgBoxError { get; } = new();

    public Interaction<string[], Unit> MsgBoxInfo { get; } = new();

    public Interaction<string[], bool> MsgBoxYesNo { get; } = new();

    public Interaction<string, Unit> AddConsoleLine { get; } = new();


    [Reactive]
    public bool IsPathsReadOnly { get; set; } = false;

    [Reactive]
    public string BanksPath { get; set; } = string.Empty;

    [Reactive]
    public string OutputPath { get; set; } = string.Empty;
    [Reactive]
    public string ProjectName { get; set; } = string.Empty;

    [Reactive]
    public string ProgressText { get; set; } = "Idle";

    [Reactive]
    public int ProgressMaximum { get; set; } = 1;

    [Reactive]
    public int ProgressValue { get; set; } = 0;
    [Reactive]
    public string BankText { get; set; } = "";

    [Reactive]
    public int BankMaximum { get; set; } = 1;

    [Reactive]
    public int BankValue { get; set; } = 0;

    public Subject<string> ConsoleLine { get; } = new();

    public ReactiveCommand<Unit, Unit> PickBanksFolderCommand { get; }
    public ReactiveCommand<Unit, Unit> PickOutputFolderCommand { get; }

    public ReactiveCommand<Unit, Unit> StartExtractingCommand { get; }

    public MainViewModel()
    {
        this.PickBanksFolderCommand = ReactiveCommand.CreateFromTask(PickBanksFolder);
        this.PickOutputFolderCommand = ReactiveCommand.CreateFromTask(PickOutputFolder);

        this.StartExtractingCommand = ReactiveCommand.CreateFromTask(StartExtracting);

        this.StartExtractingCommand.ThrownExceptions.Subscribe(async ex =>
        {
            await MsgBoxError.Handle(new string[] {
                    "ERROR!",
                    $"Error: {ex.Message}" }).ToTask();

            IsPathsReadOnly = false;
            BankText = "";
            BankValue = 0;
            BankMaximum = 1;
            ProgressText = "Idle";
            ProgressValue = 0;
            ProgressMaximum = 1;
        });

        var curDir = Environment.CurrentDirectory;

        BanksPath = Path.Combine(curDir, "Banks");
        OutputPath = Path.Combine(curDir, "Output");

        if (Directory.Exists(BanksPath) == false)
            Directory.CreateDirectory(BanksPath);

        if (Directory.Exists(OutputPath) == false)
            Directory.CreateDirectory(OutputPath);

        ProjectName = "Generic-Project";
    }

    async Task PickBanksFolder()
    {
        var path = await PickAFolder.Handle("Select Banks Folder:").ToTask();

        if (path! == string.Empty)
            return;

        BanksPath = path;
    }

    async Task PickOutputFolder()
    {
        var path = await PickAFolder.Handle("Select Folder to Output Project:").ToTask();

        if (path! == string.Empty)
            return;

        OutputPath = path;
    }

    async Task StartExtracting()
    {
        if (Directory.Exists(BanksPath) == false)
        {
            await MsgBoxError.Handle(new string[] {
                    "ERROR!",
                    "Provided Banks Path doesn't exist!" }).ToTask();
            return;
        }

        IsPathsReadOnly = true;
        BankText = "";
        BankValue = 0;
        BankMaximum = 1;
        ProgressText = "Idle";
        ProgressValue = 0;
        ProgressMaximum = 1;

        if (ProjectName == string.Empty)
        {
            var proceed = await MsgBoxYesNo.Handle(new string[] {
                    "Warning!",
                    "Project Name field is Empty! Do you still want to proceed?" }).ToTask();

            if (proceed == false)
            {
                IsPathsReadOnly = false;
                return;
            }
        }

        BankMaximum = Directory.GetFiles(BanksPath, "*.bank").Length;

        // Launch FMOD-Decompiler Console App
        var Args = $"--input {BanksPath} --output {OutputPath} --name {ProjectName} --verbose --GUI";
        var consoleappstart = ProcessX.StartAsync($"FMOD-Decompiler\\FMOD-Decompiler.exe {Args}");
        // Get Console Output
        await foreach (string consoleLine in consoleappstart)
        {
            // Add to Console
            await AddConsoleLine.Handle(consoleLine).ToTask();

            // Get Bank currently loaded
            if (consoleLine.Contains("Loaded Bank:"))
            {
                BankValue = BankValue + 1;
                BankText = consoleLine;
                ProgressValue = 0;
                ProgressMaximum = 1;
            }

            if (consoleLine.Contains("Sounds Found: ") || consoleLine.Contains("Events Found: "))
            {
                ProgressText = consoleLine;
                string[] parts = consoleLine.Split(':');
                ProgressValue = 0;
                ProgressMaximum = int.Parse(parts[1].Trim());
            }
            else if (consoleLine.Contains("Extracted Sound ") || consoleLine.Contains("Saving Event: "))
            {
                ProgressText = consoleLine;
                ProgressValue = ProgressValue + 1;
            }
        }

        IsPathsReadOnly = false;
        BankText = "";
        BankValue = 1;
        BankMaximum = 1;
        ProgressText = "Done!";
        ProgressValue = 1;
        ProgressMaximum = 1;
    }
}
