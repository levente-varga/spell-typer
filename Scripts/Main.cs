using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace SpellTyper.Scripts;

public partial class Main : Node2D {
  private Label _healthLabel;
  private Label _levelLabel;
  private Label _timeLabel;
  private Control _deathScreen;
  private Control _startScreen;
  
  private List<Word> _usedWords = [];
  private List<Word> _currentWords = [];
  private List<WordDisplay> _wordDisplays = [];
  private RandomNumberGenerator _rng = new ();

  private Timer _timer = new ();
  private int _level = 1;
  private int _enemyHealth = 0;
  
  private bool _alive = true;

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
    
    _healthLabel = GetNode<Label>("Labels/HealthValueLabel");
    _levelLabel = GetNode<Label>("Labels/LevelValueLabel");
    _timeLabel = GetNode<Label>("Labels/TimeValueLabel");
    _deathScreen = GetNode<Control>("Labels/DeathScreen");
    _startScreen = GetNode<Control>("Labels/StartScreen");
    
    _deathScreen.Hide();
    _startScreen.Show();

    AddChild(_timer);
    _timer.Timeout += OnTimeout;

    foreach (var word in _wordDisplays) {
      word.OnDone += OnWordDone;
    }
  }

  public override void _Process(double delta) {
    _timeLabel.Text = $"{(int)_timer.GetTimeLeft()}";
  }

  private void OnTimeout() {
    _alive = false;
    _timer.Stop();
    _deathScreen.Show();
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
    if (@event is InputEventKey { Keycode: Key.Enter }) {
      StartGame();
      return;
    }
    
    if (!_alive) return;
    
    base._Input(@event);
    if (@event is InputEventKey { Pressed: false }) return;
    var scancode = @event.AsText();

    if (@event is InputEventKey { Keycode: Key.Backspace }) {
      RemoveFocus();
      return;
    }
    
    if (scancode.Length is > 1 or 0) return;
    var c = scancode.ToLower()[0];

    var target = FindFocusedWord() ?? FindWordThatNeeds(c);
    if (target == null) return;

    var targetIndex = (int)target;
    _wordDisplays[targetIndex].Focused = true;
    _wordDisplays[targetIndex].Word.Type(c);
    if (IsPageDone()) TurnPage();
  }

  private bool IsPageDone() {
    foreach (var word in _wordDisplays) {
      if (word.Word == null) return true;
      if (!word.Word.Done) return false;
    }
    return true;
  }

  private int? FindWordThatNeeds(char c) {
    for (var i = 0; i < _wordDisplays.Count; i++) {
      if (_wordDisplays[i].Word.Needs(c)) return i;
    }
    return null;
  }
  
  private int? FindFocusedWord() {
    for (var i = 0; i < _wordDisplays.Count; i++) {
      if (_wordDisplays[i].Focused) return i;
    }
    return null;
  }

  private void RemoveFocus() {
    foreach (var word in _wordDisplays) {
      word.Focused = false;
    }
  }

  private void StartGame() {
    _level = 1;
    _alive = true;
    _deathScreen.Hide();
    _startScreen.Hide();
    StartLevel();
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
    for (var i = 0; i < count; i++) {
      newWords.Add(PickWordSmartly(newWords));
    }
    return newWords;
  }

  private Word PickWordSmartly(List<Word> existing) {
    var success = false;
    var maxIndex = Data.Instance.Spells.Count;
    var candidate = "";

    while (!success) {
      var index = _rng.RandiRange(0, maxIndex - 1);
      candidate = Data.Instance.Spells[index].ToLower();
      success = existing.All(word => !word.Needs(candidate[0]));
    }
    
    return new Word(candidate);
  }

  private void StartLevel() {
    _enemyHealth = (int)(Math.Sqrt(_level) * 5);
    _timer.Start(45);
    TurnPage();
    UpdateLabels();
  }
}