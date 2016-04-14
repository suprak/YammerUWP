# Universal Windows Application for Yammer
This was a side project I worked on in late 2015, with the goal of seeing how easy it was to develop a universal windows application (read: it's the easiest it's ever been).

The app is fairly feature complete. I ended up switching to it as my go-to Yammer client on Windows 10.
That being said, there's a bunch of things not implemented (ex. Polls). There's also some bugs to be found and fixed.

[Yammer REST API](https://developer.yammer.com/) is quite poorly documented and for the most part had to be reverse engineered by watching traffic go by as you use their web app.

In the process I ended up learning some cool XAML patterns and tricks.
I tried to keep the code as clean and legible as possible. You should be able to find useful things,
- Swipe left/right list items
- OAuth logic that relies on external browser (Edge) instead of custom app logic
- Adjustable layouts
- Dynamically creating clickable links in TextBlocks bound to bodies of text
- Extensibe use of x:Bind
- Custom image loading from behind protected HTTP endpoints
- Example of identical experience between Desktop and Mobile
- Nested Frame navigation model
- Image viewer logic that relies on built in Photos app instead of custom viewer
- A usable stand-alone Yammer client
- Async, async and more async
- And hopefully much more

If you want to see it in action, the app can be [downloaded](https://www.microsoft.com/store/apps/9nblggh67xss) from the store (however requires a direct link as it is hidden).

## The MIT License (MIT) 
Copyright (c) 2106 Alex Karpus

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
