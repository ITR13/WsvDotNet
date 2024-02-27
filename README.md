# WsvDotNet
### About the project
Minimalistic reader and writer for the Whitespace Separated Values format.
To read more about the format, check out [@Stenway](https://github.com/Stenway)'s [WSV documentation](https://dev.stenway.com/WSV/), or their [Videos](https://www.youtube.com/watch?v=mGUlW6YgHjE&list=PLcw9ek5nOAPHvxMq_y4ca4Po8VRnSRTPk) on the topic of table formats.  

**NB:** This implementation ignores comments! Might add comment support in the future, but wanted it as simple as possible for now.

### Usage
To use, simply copy either [WsvReader.cs](WsvDotNet%2FWsvReader.cs) or [WsvWriter.cs](WsvDotNet%2FWsvWriter.cs) into your project, and call .Read or .Write on them.
WsvReader takes a text span, so for strings you need to call .AsSpan() first.