using Godot;

namespace SpellTyper.Scripts;

public partial class Word(string word) {
  public string Typed = "";
  public string Remaining = word;
  public bool Done => Remaining.Length == 0;

  public void Type(char c) {
    if (Remaining.Length <= 0 || Remaining[0] != c) return;
    Typed = Typed + c;
    Remaining = Remaining.Remove(0, 1);
  } 
}