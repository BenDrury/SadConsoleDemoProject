﻿using System;
using System.Collections.Generic;
using System.Text;
using Crayon;

namespace SadConsole.Renderers
{
    public class ConsoleRenderer : IRenderer
    {
        public string RedrawString;
        private StringBuilder _redrawStringBuilder;

        public void Attach(ISurfaceRenderData surface)
        {
            //RedrawString = new string(' ', console.Width * console.Height);
            _redrawStringBuilder = new StringBuilder(surface.Surface.View.Width * surface.Surface.View.Height * 4);
        }

        public void Detatch(ISurfaceRenderData surface)
        {
            
        }

        public void Render(ISurfaceRenderData surface)
        {
            GameHost.Instance.DrawCalls.Enqueue(new DrawCalls.DrawCallSurface(surface.Surface, surface.AbsoluteArea.Position));            
        }

        public void Refresh(ISurfaceRenderData surface, bool force)
        {
            //_redrawStringBuilder.Clear();
            //foreach (Cell cell in console)
            //{
            //    if (cell.Glyph == 0)
            //        _redrawStringBuilder.Append(' ');
            //    else
            //        _redrawStringBuilder.Append((char)cell.Glyph);
            //}
            //RedrawString = _redrawStringBuilder.ToString();
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }


                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~ConsoleRenderer()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
