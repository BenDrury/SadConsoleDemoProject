﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SadConsole.Input;
using SadConsole.Renderers;

namespace SadConsole.Tests
{
    class BasicGameHost : GameHost
    {
        public BasicGameHost()
        {
            Instance = this;
        }

        public override IKeyboardState GetKeyboardState()
        {
            throw new NotImplementedException();
        }

        public override IMouseState GetMouseState()
        {
            throw new NotImplementedException();
        }

        public override IRenderer GetRenderer(string name)
        {
            return null;
        }

        public override ITexture GetTexture(string resourcePath)
        {
            throw new NotImplementedException();
        }

        public override ITexture GetTexture(Stream textureStream)
        {
            throw new NotImplementedException();
        }

        public override void ResizeWindow(int width, int height) => throw new NotImplementedException();

        public override void Run()
        {
            throw new NotImplementedException();
        }
    }
}
