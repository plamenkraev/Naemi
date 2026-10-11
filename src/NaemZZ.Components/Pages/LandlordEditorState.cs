using System.Text.Json;
using NaemZZ.Core.Dto;

namespace NaemZZ.Components.Pages;

/// <summary>
/// Per-page editing session. Snapshots contain complete landlord/property data.
/// No database calls or shared singleton state.
/// </summary>
public sealed class LandlordEditorState
{
    private readonly List<(string Label, string Json)> _history = [];
    private int _index = -1;
    private string? _saved;
    public LandlordDto? Current { get; private set; }
    public bool HasUnsavedChanges => Current is not null && Snapshot(Current) != _saved;
    public bool CanUndo => _index > 0;
    public bool CanRedo => _index >= 0 && _index < _history.Count - 1;
    public IReadOnlyList<string> History => _history.Select(x => x.Label).ToArray();
    public int HistoryIndex => _index;

    private static string Snapshot(LandlordDto value) => JsonSerializer.Serialize(value);
    private static LandlordDto Restore(string value) => JsonSerializer.Deserialize<LandlordDto>(value)!;

    public void Open(LandlordDto dto)
    {
        Current = dto;
        _saved = Snapshot(dto);
        _history.Clear();
        _history.Add(("Заредено", _saved));
        _index = 0;
    }

    public void Record(string label)
    {
        if (Current is null) return;
        var json = Snapshot(Current);
        if (_index >= 0 && _history[_index].Json == json) return;
        if (_index < _history.Count - 1) _history.RemoveRange(_index + 1, _history.Count - _index - 1);
        _history.Add((label, json));
        _index = _history.Count - 1;
    }

    public LandlordDto? Undo()
    {
        Record("Редакция");
        if (!CanUndo) return Current;
        Current = Restore(_history[--_index].Json);
        return Current;
    }

    public LandlordDto? Redo()
    {
        if (!CanRedo) return Current;
        Current = Restore(_history[++_index].Json);
        return Current;
    }

    public LandlordDto? JumpTo(int index)
    {
        Record("Редакция");
        if (index < 0 || index >= _history.Count) return Current;
        _index = index;
        Current = Restore(_history[index].Json);
        return Current;
    }

    public void MarkSaved()
    {
        if (Current is null) return;
        Record("Запис");
        _saved = Snapshot(Current);
    }

    public LandlordDto? Discard()
    {
        if (_saved is null) return null;
        Current = Restore(_saved);
        Record("Отказани промени");
        return Current;
    }

    public void Clear()
    {
        Current = null;
        _saved = null;
        _history.Clear();
        _index = -1;
    }
}
