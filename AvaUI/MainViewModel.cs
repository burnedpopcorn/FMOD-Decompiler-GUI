using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Reactive;
using System.Reactive.Threading.Tasks;
using System.Threading.Tasks;
using System.Reactive.Subjects;
using static FMOD_Decompiler.Program;

namespace AvaUI.ViewModels;

public class MainViewModel
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
        });
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
        await ExtractFSB(BanksPath, OutputPath, ProjectName);
    }
}
