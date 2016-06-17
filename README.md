# ImageEditor.UWP
a tool to edit image in uwp


##  Overview
this is a tool(libaray) for editing image in uwp. You can crop, rotate, select layout, apply image-filter,
tune brightness and color, add stickers onto image, add tags such as location,  drawing and etc. 

## can do
this tool has these features below:

- edit local and remote(net) images;
- select layout(1:1 3:4 4:3), select backcolor for canvas, rotate and crop the background image;
- tune brightness, degree of fuzzy and degree of sharpening;
- add stickers onto canvas(download templates from internet);
- apply a kind of filters(8 filters supported);
- add tags such as location, @friends, adn etc;
- draw on the canvas, you can select pen-size, pen-color(image) which you like;
- save the canvas as a `BitmapImage` object, which you can directly use.


## how to use
it's very simple to use this tool(libaray), you just need these code snippets below:

```
        private async void EditLocal_Tapped(object sender, TappedRoutedEventArgs e)
        {
            FileOpenPicker fo = new FileOpenPicker();
            fo.FileTypeFilter.Add(".png");
            fo.FileTypeFilter.Add(".jpg");
            fo.SuggestedStartLocation = PickerLocationId.Desktop;

            var f = await fo.PickSingleFileAsync();
            if (f != null)
            {
                // create instance and show it
                ImageEditorControl editor = new ImageEditorControl();
                editor.Show(f);

                //register event to get edited image
                editor.ImageEditedCompleted += (image_edited) =>
                {
                    image.Source = image_edited;
                };
            }
        }
```

## some screenshots
