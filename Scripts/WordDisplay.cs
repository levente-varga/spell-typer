using Godot;

namespace SpellTyper.Scripts;

public partial class WordDisplay : Control {
  private Label _typedLabel;
  private Label _remainingLabel;

  public delegate void OnDoneDelegate();
  public event OnDoneDelegate OnDone;
  
  private bool _focused;
  public bool Focused {
    set {
      _focused = value;
      OnFocusChanged();
    }
    get  => _focused;
  }
  public Word Word;

  public void AssignWord(Word w) {
    UnassignCurrentWord();
    
    Word = w;
    Word.OnChanged += OnWordChanged;
    OnWordChanged();
    OnFocusChanged();
  }
  
  private void UnassignCurrentWord() {
    if (Word == null) return;
    
    Word.OnChanged -= OnWordChanged;
    Word  = null;
  }

  private void OnWordChanged() {
    _typedLabel.Text = Word.Typed;
    _remainingLabel.Text = Word.Remaining;
    if (Word.Done) OnDone?.Invoke();
  }

  private void OnFocusChanged() {
    _remainingLabel.LabelSettings.FontColor = Focused 
      ? new Color("ffbd14ff") 
      : new Color("ffffffff");
  }

  public override void _Ready() {
    _typedLabel = GetNode<Label>("Typed");
    _remainingLabel = GetNode<Label>("Remaining");
  }
}