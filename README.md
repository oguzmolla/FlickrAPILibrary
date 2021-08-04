# FlickrAPILibrary
By using flickr api, you can do your photo throwing, deletion and listing operations.

Example delete upload listing operations using api

# Example Photo Info
```csharp
        [HttpGet("GetPhotoInfo")]
        public IActionResult GetPhotoInfo(string photoId)
        {
            FlickrManager flickr = new FlickrManager();
            var response = flickr.GetPhotoInfo(photoId);
            return Ok(response);
        }
```

# Example Photo List
```csharp
        [HttpGet("GetPhotoList")]
        public IActionResult GetPhotoList(int page, int size)
        {
            FlickrManager flickr = new FlickrManager();
            var response = flickr.GetPhotoList(page, size);
            return Ok(response);
        }
```

# Example Photo Upload
```csharp
        [HttpPut("UpdatePhoto")]
        public IActionResult UpdatePhoto([FromForm] List<IFormFile> formFiles)
        {
            FlickrManager flickr = new FlickrManager();
            flickr.GetOAuth("https://api.flickr.com/services/upload/");
            using (var ms = new MemoryStream())
            {
                formFiles?.First().CopyTo(ms);
                var response = flickr.UploadPhoto(ms.ToArray(), formFiles?.First().FileName);
                return Ok(response);
            }
        }

```

# Example Photo Delete
```csharp
        [HttpGet("DeletePhoto")]
        public IActionResult DeletePhoto(string photoId)
        {
            FlickrManager flickr = new FlickrManager();

            Dictionary<string, string> parameters = new Dictionary<string, string>();
            parameters.Add("method", "flickr.photos.delete");
            parameters.Add("photo_id", photoId);

            flickr.GetOAuth("https://www.flickr.com/services/rest/");
            var response = flickr.DeletePhoto(parameters, photoId);
            return Ok(response);
        }
 ```
