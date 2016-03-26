# Small Image Zapper
Console app to delete all small images in a given folder and all of its subfolders. Utilizes [Command Line Parser](https://github.com/gsscoder/commandline) (see examples below for usage tips) and [Taglib#](https://github.com/mono/taglib-sharp/tree/master/src). It will generally send files to the recycling bin, but you can also do a "hard delete" (which is probably preferred for massive file counts). Similarly, when run it will initially pause as a "are you sure" check, but that can be overridden so that you can utilize it in automated scripts. The simplest way to determine your preferred pixel count for "minpixels" is to simply multiply the height and width (e.g. 400 * 600 = 240,000).
# Options
All flag options are considered true if provided, false if not. They do not accept values. Skipped extensions can include the leading period or not.
```
  -f, --folder        Required. Folder path to process
  -p, --minpixels     (Default: 240000) Minimum pixels. Anything below is considered too small and will be deleted.
  -b, --maxbytes      (Default: 2147483647) Maximum file bytes for a small file. Anything larger will be ignored.
  -s, --skipext       Skip extensions (comma separated)
  -h, --harddelete    Flag for doing a hard delete instead of using recycling bin.
  -i, --immediate     Flag for skipping initial pause button prior to running. Useful for automated scripts.
  -v, --verbose       Flag for displaying all event messages
  -d, --debug         Flag for debug only mode. No files will be deleted.
  --help              Display help screen.
```
# Example Usage
```
SmallImageZapper --folder c:\somepath\xyx --skipext .gif,png
SmallImageZapper -vdf c:\somepath\xyx -p 300000 -b 150000 -s gif
SmallImageZapper -hf "c:\my test"
```
## Restrictions
If a folder path includes spaces it must be enclosed in quotes and NOT have a trailing backslash (that is, do not use something like "c:\my test\"). This is due to how Windows/.NET parses command line arguments.

# License
GNU GPL v3. See license file for more information.