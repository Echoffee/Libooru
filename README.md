# Libooru
I want to sort these thousands of pics

Another attempt to get my thousands of pictures sorted and searchable like booru-type websites.
Using WPF on VS Community.

Also using [LiteDB](https://github.com/mbdavid/LiteDB), a light noSQL embedded database which seems okay for my usage.

# Goal
A desktop application that help to sort and retrieve anime-style illustrations/artworks using tags which are taken from [Danbooru](https://danbooru.donmai.us) and [IQDB](http://danbooru.iqdb.org), as well as getting the high-definition version of said artworks if disponible. 

Because requests to IQDB are raw like being on the website form, only thumbnails are sent to the website (150x150 px, ~60kB maximum per request), I tested this method on around 300 randoms pictures taken from various sources (Facebook, Twitter, etc..., but not cropped pictures) and results were satisfying. Only the first big scan on the picture library should take some time and bandwidth.
 
# Current status and TODO list

| Item                                        |🔨| Notes                                                                      |
|---------------------------------------------|--|----------------------------------------------------------------------------------|
|Display pictures in a booru-like layout      |✅|
|Display complete informations on pictures    |🔳| Only tags and picture are displaying for now, also tags are not in a proper order
|Config file and database support             |🔳|Almost done with this
|General informations about the database      |  |
|Directories informations and configuration   |🔳| Missing "Scan on startup" feat
|Tags informations and configuration          |  |
|Third party support and configuration        |  | Only Danbooru support for now
|File scanning                                |🔳| *Huge* RAM usage on full scan, also related to "Scan on startup" feat
|Thumbnail generation                         |✅|
|File renaming options                        |  |
|Picture tagging                              |✅| Rares errors comming from IQDB while scrapping tho
|Picture navigation like any gallery software |  | Can only display in HD without zoom and stuff
|Link to artist's website (Pixiv, etc...)     |  |
|Get HD ver. of a picture                     |  |
|Search engine                                |  |

:white_square_button: : Work in progress.

:white_check_mark: Done.

# Usage
There is currently no support for using the software beside compiling the whole solution in Visual Studio.

# Modified environment outside project directories
A directory named `Libooru` is created in `C:\Users\...\Documents` containing the config file and database. Deleting it *while* using the application will probably results in a crash. Restarting the application will recreate the files as it was used for the first time.
