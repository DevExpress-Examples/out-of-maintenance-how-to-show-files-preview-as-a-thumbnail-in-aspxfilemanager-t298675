<!-- default file list -->
*Files to look at*:

* [Default.aspx](./CS/Default.aspx) (VB: [Default.aspx](./VB/Default.aspx))
* [Default.aspx.cs](./CS/Default.aspx.cs) (VB: [Default.aspx.vb](./VB/Default.aspx.vb))
<!-- default file list end -->
# How to show files preview as a thumbnail in ASPxFileManager


<p>This example is a simplified implementation of the DXDocs demo that illustrates how to create a file thumbnail based on the first page of the document. <br>The <a href="https://documentation.devexpress.com/#AspNet/DevExpressWebASPxFileManager_CustomThumbnailtopic">ASPxFileManager.CustomThumbnail</a> event is used for defining a custom value for the <a href="https://documentation.devexpress.com/#AspNet/DevExpressWebFileManagerItem_ThumbnailUrltopic">FileManagerItem.ThumbnailUrl</a> property. Since this solution does not provide a cashing mechanism, previews are generated on each request. To increase application performance, refer to the <em>FileSystemService</em> class of the DXDocs demo.</p>
<p><strong>Important Note:</strong></p>
<p>The <strong>Document Server</strong> product license is required for using this approach. Please refer to the <a href="https://www.devexpress.com/Subscriptions/"><u>Subscriptions</u></a> page for more information.</p>

<br/>


