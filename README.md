# RoundedButtons
Simple tool to help create rounded buttons on your WinForms.

I've included color and line styles for Normal, Disabled, Highlighted, and Clicked buttons.
Everything can be set when creating a new RoundedButtons object.
Then just use that object to paint the buttons and subscribe to the events automatically.


Example Script:

    RoundedButtons roundedButtons = new RoundedButtons()
      { Btn_CornerRadius = 5, Btn_ShadowWidth = ShadowSize.Thick };
      
    roundedButtons.PaintButton(button1);
  
Example for lots of buttons:

    RoundedButtons roundedButtons2 = new RoundedButtons();
    
    foreach (object o in panel1.Controls)
    {
      if (o is Button b)
      {
        roundedButtons2.PaintButton(b);
      }
    }
