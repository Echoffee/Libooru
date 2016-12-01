# Libooru
I want to sort these thousands of pics

Another attempt to get my thousands of pictures sorted and searchable like booru-type websites.
Using WPF on VS Community.

Also using [LiteDB](https://github.com/mbdavid/LiteDB), a light noSQL embedded database which seems okay for my usage.

# Objective
A desktop application that help to sort and retrieve anime-style illustrations/artworks using tags which are taken from [Danbooru](https://danbooru.donmai.us) and [IQDB](http://danbooru.iqdb.org), as well as getting the high-definition version of said artworks if disponible. 

Because requests to IQDB are raw like being on the website form, only thumbnails are sent to the website (150x150 px, ~60kB maximum per request), I tested this method on around 300 randoms pictures taken from various sources (Facebook, Twitter, etc..., but not cropped pictures) and results were satisfying. Only the first big scan on the picture library should take some time and bandwidth.
 
