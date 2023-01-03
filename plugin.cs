// Name: Isometric Cuboid Creator
// Submenu: Isometric
// Author: Sefhriloff
// Title: Isometric Cuboid Creator
// Version: 1
// Desc:
// Keywords:
// URL:
// Help:

#region UICode
IntSliderControl _frontalSizeControl = 10; // [1,999] Frontal Size
IntSliderControl _lateralSizeControl = 10; // [1,999] Lateral Size
IntSliderControl _height = 20; // [0,999] Height
PanSliderControl _slider = Pair.Create(0.000,0.000); // Location
CheckboxControl _useColor = true; // Use Color
ColorWheelControl _color = ColorBgra.FromBgr(0, 0, 255); // Color
#endregion

void DrawPixel(int x, int y, ColorBgra color, Surface dst, Rectangle rect){
     if(x > rect.Right-1 || y > rect.Bottom-1 || x < 0 || y < 0) return;
        dst[x,y] = color;
}

ColorBgra getOffsetColor(ColorBgra old, int offset){
    int newR = _color.R + offset;
    if(newR > 255) newR = 255;
    if(newR < 0) newR = 0;

    int newG = _color.G + offset;
    if(newG > 255) newG = 255;
    if(newG < 0) newG = 0;
    
    int newB = _color.B + offset;
    if(newB > 255) newB = 255;
    if(newB < 0) newB = 0;

    return ColorBgra.FromBgr((byte)newB, (byte)newG, (byte)newR);
}

void Render(Surface dst, Surface src, Rectangle rect)
{
    Rectangle selection = EnvironmentParameters.SelectionBounds;
    int centerX = ((selection.Right - selection.Left) / 2) + selection.Left;
    int centerY = ((selection.Bottom - selection.Top) / 2) + selection.Top;
    ColorBgra primaryColor = EnvironmentParameters.PrimaryColor;
    ColorBgra secondaryColor = EnvironmentParameters.SecondaryColor;
    int brushWidth = (int)EnvironmentParameters.BrushWidth;

    if(IsCancelRequested) return;

    // Clear Previous Change
    dst.CopySurface(src);

    int _frontalSize = _frontalSizeControl * 2;
    int _lateralSize = _lateralSizeControl * 2;

    // Adding the offset to the rectangle position
    int startX = centerX + (int)(_slider.First * 100);
    int startY = centerY + (int)(_slider.Second * 100);
    startX = (startX % 2 == 0) ? startX : startX +1;

    //Calculate Rectangle Colors. Top is lighter!
    ColorBgra darkerColor = getOffsetColor(_color, -100);
    ColorBgra outlineColor = (_useColor) ? getOffsetColor(_color, 60) : ColorBgra.Black;
    ColorBgra topColor = getOffsetColor(_color, 25);

    // Calculate Points. Yeah im lost :(
    int[] pointA = new int[]{startX, startY};
    int[] pointB = new int[]{startX + _frontalSize, startY + (_frontalSize / 2)};
    int[] pointC = new int[]{pointB[0] + _lateralSize, pointB[1] - (_lateralSize / 2)};

    //Draw Isometric Frontal Line
    int pX = pointA[0], pY = pointA[1];
    for(int ln1 = 0; ln1 < _frontalSize; ln1++){
        if(pX % 2 == 0) pY++;
        pX++;

      DrawPixel(pX, pY, outlineColor, dst, rect);
      DrawPixel((pX + _lateralSize) - 2, (pY - (_lateralSize / 2)) + 1,ColorBgra.Black, dst, rect);
     
      // Im using color option, so fill this trash!
      if(_useColor){
          for(int ix = 1; ix < _height; ix++){
          DrawPixel(pX, pY + ix, _color, dst, rect);
          }
      }

      DrawPixel(pX, pY + _height, ColorBgra.Black, dst, rect);
    }

    //Draw Isometric Lateral Line 
    pX = pointB[0];
    pY = pointB[1];
    for(int ln2 = 0; ln2 < _lateralSize; ln2++){

        if(pX % 2 == 0) pY--;
        pX++;

      if(ln2 < _lateralSize -2) DrawPixel(pX - 2, pY + 1, outlineColor, dst, rect);
      DrawPixel(pX - _frontalSize, pY - (_frontalSize / 2) + 2,ColorBgra.Black, dst, rect);
     
      // Im using color option, so fill this trash!
      if(_useColor){
        for(int ix = 1; ix < _height; ix++){
          if(ln2 > _lateralSize -4) break;
          DrawPixel(pX, pY + ix, darkerColor, dst, rect);
        }
      }

      DrawPixel(pX - 2, (pY + 1) + _height,ColorBgra.Black, dst, rect);
    }

    // Draw Lateral Lines
    int fix = (_frontalSize < 4) ? -1 : 0;
    for(int height = 1; height < _height ; height++){
        DrawPixel(pointB[0] + fix, pointB[1] + height, outlineColor, dst, rect);
        DrawPixel(pointA[0] + 1, pointA[1] + height +1, ColorBgra.Black, dst, rect);
        DrawPixel(pointC[0] - 2, pointC[1] + height +1, ColorBgra.Black, dst, rect);
    }

    // Add Color to the Rectangle top
    if(_useColor){
        for(int lnc = 0; lnc < (_lateralSize / 2)-1; lnc++){
             pX = (pointA[0] + (lnc * 2) +2);
             pY = (pointA[1] - lnc);
            for(int nwln = 0; nwln < _frontalSize-2; nwln++){
               if(pX % 2 == 0) pY++;
                pX++;
                DrawPixel(pX, pY, topColor, dst, rect);
                if(lnc == (_lateralSize / 2)-2) continue;
                if(nwln < _frontalSize-4) DrawPixel(pX+2, pY, topColor, dst, rect);
            }
        }
    }
}
