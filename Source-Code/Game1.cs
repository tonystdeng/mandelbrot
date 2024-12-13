using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Collections.Generic;

namespace mandelbrot;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private SpriteFont font;
    Texture2D pixel;
    float[,] map;
    double fps;
    double zoom=1,locx=0,locy=0;
    double scale=0;int size=0;
    double temp;
    bool elf=true;//stand for event last flip

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        IsFixedTimeStep = false;
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here
        _graphics.PreferredBackBufferWidth = 1200;
        _graphics.PreferredBackBufferHeight = 1200;
        map=new float[_graphics.PreferredBackBufferWidth,_graphics.PreferredBackBufferHeight];
        scale=0.5;
        _graphics.ApplyChanges();
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        pixel = new Texture2D(GraphicsDevice, 1, 1);
        Color[] data = new Color[1];
        data[0] = Color.White; // Set the color of the pixel
        pixel.SetData(data);
        font=Content.Load<SpriteFont>("Font");
    }

    protected override void Update(GameTime gameTime)
    {
        Event();
        if (elf){
            size=(int)(_graphics.PreferredBackBufferWidth*scale)-1;
            for (int x=0;x<size;x++){
                for (int y=0;y<size;y++){
                    int localX = x;
                    int localY = y;
                    map[y,x]=checkPoint(4.0/zoom/size*localX-2+locx,
                                    4.0/zoom/size*localY-2-locy);
                }
            }
            elf=false;
        }
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.White);
        _spriteBatch.Begin();
        fps=1000.0/gameTime.ElapsedGameTime.TotalMilliseconds;
        for (int x=0;x<size;x++){
            for (int y=0;y<size;y++){
                Color color=new Color(0,0,0);
                if (map[y,x]<1){
                    int green=(int) (map[y,x]*255);
                    if (map[y,x] > 0.3) {color=new Color(green, 255, green);}
                    else {color=new Color(10,green*3,10);}
                }
                _spriteBatch.Draw(pixel, new Rectangle(
                    new Point((int)(x/scale)+1, (int)(y/scale)),
                    new Point((int)(1/scale)+1,(int)(1/scale)+1)
                    ),color);
            }
        }
        _spriteBatch.DrawString(font, $"fps: {fps}", new Vector2(0,0), Color.Red);
        _spriteBatch.DrawString(font, $"resolusion: {(int)(scale*100)}%(right/left to scale)", new Vector2(0,24), Color.Red);
        _spriteBatch.DrawString(font, $"zoom: {zoom}x, y: {locy} x: {locx}", new Vector2(0,48), Color.Red);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    protected void Event()
    {
        KeyboardState keyboardState = Keyboard.GetState();
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed){Exit();}
        if (keyboardState.IsKeyDown(Keys.Right) && scale<=1)
        {
            scale+=0.01;
        }
        else if (keyboardState.IsKeyDown(Keys.Left) && scale>0.02)
        {
            scale-=0.01;elf=true;
        }
        if (keyboardState.IsKeyDown(Keys.Up)){
            temp=4/zoom;
            zoom*=1+0.2/fps;
            temp=(temp-4/zoom)/2;
            locx+=temp;locy-=temp;elf=true;
        }else if (keyboardState.IsKeyDown(Keys.Down)){
            temp=4/zoom;
            zoom*=1-0.2/fps;
            temp=(temp-4/zoom)/2;
            locx+=temp;locy-=temp;elf=true;
        }if (keyboardState.IsKeyDown(Keys.W)){locy+=1/fps/zoom;elf=true;}
        if (keyboardState.IsKeyDown(Keys.D)){locx+=1/fps/zoom;elf=true;}
        if (keyboardState.IsKeyDown(Keys.S)){locy-=1/fps/zoom;elf=true;}
        if (keyboardState.IsKeyDown(Keys.A)){locx-=1/fps/zoom;elf=true;}

    }

    private static float checkPoint(double x,double y)
    {
        int itor=500;
        double real,img,initX=x,initY=y;
        for (int i=0;i<itor;i++){//loop 500 times see if it goes out of 2 radius
            double lfc=x*x+y*y;//check if out of range
            if (lfc>4){//if so, end the loop
                return (float)(i-1)/(float)itor;
            }
            real=x*x-y*y+initX;     
            img=2*x*y+initY;
            x=real;y=img;
        }return 1;
        //return rand.Next(0,2)!=0;
    }
}
