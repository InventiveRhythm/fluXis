using System;
using System.Collections.Generic;
using fluXis.Online.API.Models.Maps.Modding;
using fluXis.Online.API.Models.Users;
using fluXis.Online.Fluxel;
using JetBrains.Annotations;
using osu.Framework.Allocation;
using osu.Framework.Graphics.Containers;

namespace fluXis.Screens.Edit.Modding;

public partial class EditorModding : CompositeComponent
{
    [Resolved]
    private IAPIClient api { get; set; }

    private readonly List<EditorChangeRequest> requests = new();
    public IReadOnlyList<EditorChangeRequest> Requests => requests;

    public bool IsActive { get; private set; }
    public Type ViewType { get; private set; }

    [CanBeNull]
    public APIModdingAction CurrentAction { get; private set; }

    public event Action OnEnable;
    public event Action OnDisable;

    private void addAction(APIModdingAction action)
    {
        if (action.State >= APIModdingActionState.Resolved || action.ChangeRequests is null)
            return;

        foreach (var request in action.ChangeRequests)
            requests.Add(new EditorChangeRequest(action, request));
    }

    public void Enable()
    {
        addAction(new APIModdingAction("aa", api.User.Value ?? APIUser.Default, APIModdingActionType.Note, APIModdingActionState.Pending, "change this", 0)
        {
            ChangeRequests = new List<APIModdingChangeRequest>
            {
                new()
                {
                    StartTime = 576,
                    CommentContent = "wuh"
                },
                new()
                {
                    StartTime = 2307,
                    EndTime = 3076,
                    HexColor = "#55ff55",
                    CommentContent = "this fucking sucks"
                }
            }
        });

        CurrentAction = new APIModdingAction(
            "temp",
            api.User.Value ?? APIUser.Dummy,
            APIModdingActionType.Note,
            APIModdingActionState.Pending,
            "", 0)
        {
            ChangeRequests = new List<APIModdingChangeRequest>()
        };

        ViewType = Type.Purifier;
        IsActive = true;
        OnEnable?.Invoke();
    }

    public void Disable()
    {
        IsActive = false;
        requests.Clear();
        OnDisable?.Invoke();
    }

    public void Toggle()
    {
        if (IsActive) Disable();
        else Enable();
    }

    public enum Type
    {
        View,
        Mapper,
        Purifier
    }
}
