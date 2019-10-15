# Ascend 2019 PWA Lab

In this lab we will be demonstrating how to build a progressive web app into your Episerver site. We will be demoing on Episerver's Mosey example site.

[WHAT IS PWA BLURB]

## Step 1: Adding a manifest file
The manifest file is a JSON file that tells the browser about your app and describes its functionality once installed on your mobile device. We will be using the example manifest below.

```JSON
{
  "dir": "ltr",
  "lang": "en",
  "name": "Inspiration",
  "scope": "/inspiration/",
  "display": "standalone",
  "start_url": "https://pwademo-dev-app.azurewebsites.net/inspiration/",
  "short_name": "Inspiration",
  "theme_color": "transparent",
  "background_color": "transparent",
  "description": "",
  "orientation": "any",
  "related_applications": [],
  "prefer_related_applications": false,
  "generated": "true",
  "icons": [
    {
      "src": "Assets/Images/icons/icon-72x72.png",
      "sizes": "72x72",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-96x96.png",
      "sizes": "96x96",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-128x128.png",
      "sizes": "128x128",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-144x144.png",
      "sizes": "144x144",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-152x152.png",
      "sizes": "152x152",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-192x192.png",
      "sizes": "192x192",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-384x384.png",
      "sizes": "384x384",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-512x512.png",
      "sizes": "512x512",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-180x180.png",
      "sizes": "180x180",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-120x120.png",
      "sizes": "120x120",
      "type": "image/png"
    },
    {
      "src": "Assets/Images/icons/icon-167x167.png",
      "sizes": "167x167",
      "type": "image/png"
    }
  ],
  "splash_pages": null
}
```

A couple of properties to take note of:

* __scope__ - restricts the PWA to a specific section of your site.
* __display__ - This tells the PWA how to render. Because our site is response, we will choose "standalone" to give it a native app look and feel.
* __start_url__ - This will be the first page that loads in your PWA.
* __icons__ - These icons will be used for your home screen, splash screen, etc. Chrome recommends having at least 192x192 and 512x512.

We need to add the manifest file to our web folder's root. We also need to reference to the manifest in the markup.

```html
<link rel='manifest' href='/manifest.json'>
```

You can find this markup in the _LayoutMosey.cshtml file, where it is currently commented out. 

## Step 2: Add the service workers

The service worker is a script that runs in the background, separate from your web page. It can handle things like push notifications, background sync, managing caching and responses.

For our lab, we have a [sevice worker](https://github.com/nansen/PWA-Ascend-2019/blob/master/Sources/EPiServer.Reference.Commerce.Site/pwabuilder-sw.js) already setup. What we will need to do is register it. 

This service worker has very basic functionality. It caches an "offline" page, that will be shown to the end user if they lose connection to the internet.

More complex service workers can be setup with more complex caching policies. Some examples along with service worker scripts are found at this [pwa builder site](https://www.pwabuilder.com/serviceworker).

## Step 3: Register the service worker

To use the service worker, we must first register it. Again, a service worker script was already [created for you](https://github.com/nansen/PWA-Ascend-2019/blob/master/Sources/EPiServer.Reference.Commerce.Site/pwabuilder-sw-register.js). We just need to add it to our markup. 

```html
<script src="~/pwabuilder-sw-register.js" async></script>
```

You can find this markup in the _LayoutMosey.cshtml file, where it is currently commented out.

Things to take note of in this script:
* It asks us to specify the path to our service worker script. We have it in the site root along with this file.
* It asks us to specify its scope. Just as we did in the manifest, we restrict it to the "inspiration" section of the site.

## Step 4: Add the offline.html file

This step is straight forward. The service work has the ability to cache an offline file. This file will be displayed to the user when they lose connection. We want to add that file to the solution.

We added this [offline file](https://github.com/nansen/PWA-Ascend-2019/blob/master/Sources/EPiServer.Reference.Commerce.Site/offline.html) for you, but we wanted to call it out as it's own step. It is a basic html file, and you can customize it for your application.

You can see where this is referenced, at the top of the pwabuilder-sw.js.

## Step 5: Adding Icons

As we did with the last step, this step was already completed and is pretty straight forward. We wanted to call it out because it is important make sure you have the correct icon sizes. As a PWA, we do not know what type of device your app will be installed on. We want to make sure it will render on all devices. [Google recommends](https://developers.google.com/web/tools/lighthouse/audits/install-prompt#recommendations) using at least 192x192 and 512x512 as I mentioned above. We included all the recommended Android and iOS icon sizes to handle most devices.

## Testing

If you navigate to the [inspiration](https://pwademo-dev-app.azurewebsites.net/inspiration) section of the site on a mobile device, there are two ways to install it as an app.

1. On iOS devices, if you click the share button, then select the "Add to Home Screen" option, it will show a modal. Here you can set the name and link for the app, which it defaults to the values specified in the manifest. From there you click "Add", and it will be added to your home screen like any other app.

2. For Android devices, it will depend on the browser you are using. But you should be able to "Add to Homescreen" in a similar fashion as iOS devices.

If you have any issues with your application, you can go to the "Audits" tab in Chrome tools. There is an option for auditing your site as a PWA. It will catch potential issues and help with debugging.

You can also go to the "Application" section of Chrome tools. There it will list any manifest files and service workers associated with a the site.
