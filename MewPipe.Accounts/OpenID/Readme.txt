This code is not my own, it comes from this repository : https://github.com/RockstarLabs/OwinOAuthProviders


It was a project with a lot of Authentication providers for OWIN (ASP.NET Authentication), there was an OpenID middleware, but it was limited to one authority (Yahoo OR Google but not both for example)

I modified a small part of the projet in order to be able to use it with every openID provider supported by the projet by providing the URL in a form.

You'll be able to see the modifications in the OpenIDAuthenticationHandler.cs file at the lines 332 to 337.


This project is licensed by the MIT Licence you'll find it here :

============================================================================
The MIT License (MIT)

Copyright (c) 2014 Jerrie Pelser

Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files (the "Software"), to deal in the Software without restriction, including without limitation the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
============================================================================