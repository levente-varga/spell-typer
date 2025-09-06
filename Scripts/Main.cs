using System;
using System.Collections.Generic;
using Godot;

namespace SpellTyper.Scripts;

public partial class Main : Node2D {
  private Label _healthLabel;
  private Label _levelLabel;
  
  private List<Word> _usedWords = [];
  private List<Word> _currentWords = [];
  private List<WordDisplay> _wordDisplays = [];
  private RandomNumberGenerator _rng = new ();

  private Timer _timer = new ();
  private int _level = 1;
  private int _enemyHealth = 0;

  public override void _Ready() {
    _wordDisplays.AddRange([
      GetNode<WordDisplay>("Book/Pages/Left/Words/Row1"),
      GetNode<WordDisplay>("Book/Pages/Left/Words/Row2"),
      GetNode<WordDisplay>("Book/Pages/Left/Words/Row3"),
      GetNode<WordDisplay>("Book/Pages/Left/Words/Row4"),
      GetNode<WordDisplay>("Book/Pages/Right/Words/Row1"),
      GetNode<WordDisplay>("Book/Pages/Right/Words/Row2"),
      GetNode<WordDisplay>("Book/Pages/Right/Words/Row3"),
      GetNode<WordDisplay>("Book/Pages/Right/Words/Row4"),
    ]);
    
    _healthLabel = GetNode<Label>("HealthValueLabel");
    _levelLabel = GetNode<Label>("LevelValueLabel");

    AddChild(_timer);

    foreach (var word in _wordDisplays) {
      word.OnDone += OnWordDone;
    }
    
    StartLevel();
  }

  public override void _Process(double delta) {
    
  }

  private void OnWordDone() {
    _enemyHealth--;
    if (_enemyHealth <= 0) {
      _level++;
      StartLevel();
    }
    UpdateLabels();
  }

  private void UpdateLabels() {
    _healthLabel.Text = $"{_enemyHealth}";
    _levelLabel.Text = $"{_level}";
  }

  public override void _Input(InputEvent @event) {
    base._Input(@event);
    var scancode = @event.AsText();
    if (scancode.Length is > 1 or 0) return;
    var c = scancode[0];
    GD.Print(c);
    var wordIndex = FindWordThatNeeds(c);
    if (wordIndex < 0) {
      GD.Print("No one wanted this");
      return;
    }
    GD.Print($"#{wordIndex} want this");
    _wordDisplays[wordIndex].Word.Type(c);
    if (IsPageDone()) TurnPage();
  }

  private bool IsPageDone() {
    foreach (var word in _wordDisplays) {
      if (word.Word == null) return true;
      if (!word.Word.Done) return false;
    }
    return true;
  }

  private int FindWordThatNeeds(char c) {
    for (var i = 0; i < _wordDisplays.Count; i++) {
      if (_wordDisplays[i].Word.Needs(c)) return i;
    }
    return -1;
  }

  private void TurnPage() {
    _usedWords.AddRange(_currentWords);
    _currentWords.Clear();
    var words = GetNewWords(_wordDisplays.Count);
    for (var i = 0; i < _wordDisplays.Count; i++) {
      _wordDisplays[i].AssignWord(words[i]);
    }
  }

  private List<Word> GetNewWords(int count) {
    var newWords = new List<Word>();
    var maxIndex = Data.Instance.Spells.Count;
    for (var i = 0; i < count; i++) {
      var index = _rng.RandiRange(0, maxIndex);
      newWords.Add(new Word(Data.Instance.Spells[index]));
    }
    return newWords;
  }

  private void StartLevel() {
    _enemyHealth = (int)(Math.Sqrt(_level) * 5);
    _timer.Start(30);
    TurnPage();
    UpdateLabels();
  }
}