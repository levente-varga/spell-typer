using System.Collections.Generic;
using Godot;

namespace SpellTyper.Scripts;

public partial class Main : Node2D
{
  private List<Word> usedWords = [];
  private List<Word> currentWords = [];

  private int Level = 1;
  private int enemyHealth = 0;
  
  public override void _Ready()
  {}
}