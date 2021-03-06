﻿/* 
 * FigmaImageView.cs - NSImageView which stores it's associed Figma Id
 * 
 * Author:
 *   Jose Medrano <josmed@microsoft.com>
 *
 * Copyright (C) 2018 Microsoft, Corp
 *
 * Permission is hereby granted, free of charge, to any person obtaining
 * a copy of this software and associated documentation files (the
 * "Software"), to deal in the Software without restriction, including
 * without limitation the rights to use, copy, modify, merge, publish,
 * distribute, sublicense, and/or sell copies of the Software, and to permit
 * persons to whom the Software is furnished to do so, subject to the
 * following conditions:
 * 
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
 * OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
 * MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN
 * NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM,
 * DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR
 * OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE
 * USE OR OTHER DEALINGS IN THE SOFTWARE.
 */

using System.Collections.Generic;
using Xamarin.Forms;

namespace ClippySharp
{
	public class EmptyView : View
	{

	}

	public class ViewWrapper : IViewWrapper
    {
        public object NativeObject => nativeView;

        public IViewWrapper Parent
        {
            get
            {
                if (nativeView.Parent != null)
                {
                    return new ViewWrapper(nativeView.Parent as View);
                }
                return null;
            }
        }

        readonly List<IViewWrapper> children = new List<IViewWrapper>();
        public IReadOnlyList<IViewWrapper> Children => children;

        public float X
        {
            get => (float)nativeView.X;
            set
            {
				//layer.Bounds = new CoreGraphics.CGRect(0, 0, 150, 150);
				//Position = new CGPoint(50, 50);
				//nativeView.Frame = new CoreGraphics.CGRect(value, nativeView.Frame.Y, nativeView.Frame.Width, nativeView.Frame.Height);
			}
        }
        public float Y
        {
            get => (float)nativeView.Y;
            set
            {
                //nativeView.Frame = new CoreGraphics.CGRect(nativeView.Frame.X, value, nativeView.Frame.Width, nativeView.Frame.Height);
            }
        }
        public float Width
        {
            get => (float)nativeView.Width;
            set
            {
              //  nativeView.Bounds = new CoreGraphics.CGRect(0,0, value, nativeView.Frame.Height); 
              //  nativeView.Frame = new CoreGraphics.CGRect(nativeView.Frame.X, nativeView.Frame.Y, value, nativeView.Frame.Height);
            }
        }
        public float Height
        {
            get => (float)nativeView.Height;
            set
            {
                //nativeView.Bounds = new CoreGraphics.CGRect(0, 0, nativeView.Frame.Width, value);
                //nativeView.Frame = new CoreGraphics.CGRect(nativeView.Frame.X, nativeView.Frame.Y, nativeView.Frame.Width, value);
            }
        }

        public string Identifier { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public string NodeName { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }
        public bool Hidden { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

		protected View nativeView;

        public ViewWrapper() : this (new EmptyView ())
        {

        }

        public virtual void ClearSubviews()
        {
            //clean views from current container
            //var views = nativeView.Subviews;
            //foreach (var item in views)
            //{
            //    item.RemoveFromSuperview();
            //}
            //nativeView.RemoveConstraints(nativeView.Constraints);

            //Figma doesn't calculate the bounds of our first level
            //frameEntityResponse.FigmaMainNode.CalculateBounds();

        }

        public ViewWrapper(View nativeView)
        {
            this.nativeView = nativeView;
        }

        public virtual void AddChild(IViewWrapper view)
        {
            children.Add(view);

            //nativeView.AddSubview(view.NativeObject as View);
        }

        public virtual void RemoveChild(IViewWrapper view)
        {
            if (children.Contains (view))
            {
                children.Remove(view);
               // ((View)view.NativeObject).RemoveFromSuperview();
            }
        }

        public void MakeFirstResponder()
        {
            //if (nativeView.CanBecomeFirstResponder)
            //{
            //    nativeView.BecomeFirstResponder();
            //}
        }
    }
}
