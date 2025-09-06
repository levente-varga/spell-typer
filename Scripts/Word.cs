namespace SpellTyper.Scripts;

public class Word(string word) {
  public string Typed = "";
  public string Remaining = word.ToLower();
  public bool Done => Remaining.Length == 0;
  
  public bool Needs(char c) => !Done && Remaining[0] == c;
  
  public delegate void OnChangedDelegate();
  public event OnChangedDelegate OnChanged;

  public void Type(char c) {
    if (!Needs(c)) return;
    Typed = Typed + c;
    Remaining = Remaining.Remove(0, 1);
    
    OnChanged?.Invoke();
  } 
}