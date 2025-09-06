using System;
using System.Collections.Generic;
using Godot;

namespace SpellTyper.Scripts;

public partial class Main : Node2D
{
  private List<Word> usedWords = [];
  private List<Word> currentWords = [];
  private RandomNumberGenerator rng = new ();

  private Timer timer = new ();
  private int level = 1;
  private int enemyHealth = 0;
  
  public override void _Ready()
  {}

  private void TurnPage() {
    usedWords.AddRange(currentWords);
    currentWords.Clear();
    currentWords = GetNewWords();
  }

  private List<Word> GetNewWords() {
    var newWords = new List<Word>();
    var maxIndex = Data.Instance.Spells.Count;
    for (var i = 0; i < 8; i++) {
      var index = rng.RandiRange(0, maxIndex);
      newWords.Add(new Word(Data.Instance.Spells[index]));
    }
    return newWords;
  }

  private void StartLevel() {
    enemyHealth = (int)(Math.Sqrt(level) * 5);
    timer.Start(30);
    TurnPage();
  }
}