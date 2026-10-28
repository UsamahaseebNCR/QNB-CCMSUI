
    var autoUpdateContent=false;
    var showEmptyRegion=false;
    
    function GetSelectedNode()
    {	return ATMTreeView.get_selectedNode(); }
    
    function initFrame()
 	{
  		
  		document.getElementById('contentFrame').style.height = window.screen.availHeight - 273; 	
 		//alert(window.screen.availHeight - 313);			 		
  	}



function DeleteNode(id)
    {
    var n = ATMTreeView.findNodeById("r"+id);
    n.remove();
    
    }
  	
  	
function DeleteATM(id)
    {
    var n = ATMTreeView.findNodeById("a"+id);
    n.remove();
    
    }
      	
  	
    function init()
    {
        set_autoUpdateContent();//autoUpdateContent);
        set_showEmptyRegion();        
    }
    function set_showEmptyRegion(arg)
    {
        if (arg == undefined)
            set_showEmptyRegion(!showEmptyRegion);
        else if (arg)
        {
            document.getElementById('showEmptyRegion_button').src = 'images/home.gif'; 
            showEmptyRegion = arg;
        }
        else
        {
            document.getElementById('showEmptyRegion_button').src = 'images/icon_err.gif'; 
            showEmptyRegion = arg;
        }
    }
    function set_autoUpdateContent(arg)
    {
        if (arg == undefined)
            set_autoUpdateContent(!autoUpdateContent);
        else if (arg)
        {
            // change image to pressed button image etc
            document.getElementById('autoUpdateContent_button').src = 'images/home.gif'; 
            autoUpdateContent = arg;
        }
        else
        {
            // change image to un-pressed button image etc
            document.getElementById('autoUpdateContent_button').src = 'images/icon_err.gif'; 
            autoUpdateContent = arg;
        }
    }
    
    function ReloadTree()
    {
        alert('yet to do : tree reload');
    }
    var contentPage;
    var panel_width = 0;
    var timer;
    function SetContentPage(cp)
    {  contentPage = cp; }
    
    function OnTreePanelResized(sender, args)
    { 
    try{
        if (timer) clearTimeout(timer);
        panel_width = args.get_pane().get_width();
        
        timer = window.setTimeout("setPanelWidth()",1000);
        }catch(e){ alert('Error: '+e.message);}
    }
    
    function setPanelWidth()
    {   CallBack_TreePanelWidthChange.Callback(panel_width);    }
    
    function NewRegionProvided(regionName)
    {  
        if (ATMTreeView.get_selectedNode())
            Callback_RegionAdd.Callback(regionName, ATMTreeView.get_selectedNode().get_id());
        else
            Callback_RegionAdd.Callback( regionName, null );
        
        return true;
    }
    
    function RenameATM(atmName,ATMID)
    {
        var n = ATMTreeView.findNodeById('a'+ATMID);        
        n.set_text(atmName);
        ATMTreeView.render();
    }       
    
    
    function RenameRegion(Name,ID)
    {
        
        var n = ATMTreeView.findNodeById('r'+ID);        
        n.set_text(Name);
        ATMTreeView.render();
    }       
    
    
    function AddATM(atmName,ATMID)
    {
        var nodes;
        var n = new ComponentArt.Web.UI.TreeViewNode(); 
        n.set_text(atmName);
        
        nodes = ATMTreeView.get_selectedNode().get_nodes();
      
        n.set_id('a'+ATMID);
        n.set_imageUrl('icon_ATM.jpg');
        nodes.add(n);
        
        ATMTreeView.render();        
        var newitem = new Array();
        newitem[0] = n.get_id();
        newitem[1] = n.get_text();
        ary.push(newitem);
        //ary[ary.length] = newitem;        
        
   //     ShowSuccess('Region ' + regionName + ' was added.');
    }
    
    
//    function AddOrganization(organizationName,, parent, organizationID, editable)
//    {
//        var nodes;
//        var n = new ComponentArt.Web.UI.TreeViewNode(); 
//        n.set_text(organizationName);
//        
//        if (parent == 'null')   
//            nodes = ATMTreeView.get_nodes();            
//        else// if (parent.charAt(0) == 'r')     
//            nodes =ATMTreeView.findNodeById(parent).get_nodes();
//      
//        n.set_id('o'+newid);
//        n.set_imageUrl('icon_region.jpg');
//        n.set_editingEnabled(editable); 
//        nodes.add(n);
//        
//        ATMTreeView.render();        
//        var newitem = new Array();
//        newitem[0] = n.get_id();
//        newitem[1] = n.get_text();
//        
//        ary.push(newitem);
//        ShowSuccess('Organization ' + regionName + ' was added.');
//    }       
//    
//           
//     
//     
    function AddNode(regionName, parent, newid, editable, isOrganization)
    {
        
        var nodes;
        var n = new ComponentArt.Web.UI.TreeViewNode(); 
        n.set_text(regionName);
        
        if (parent == 'null')   
            nodes = ATMTreeView.get_nodes();            
        else// if (parent.charAt(0) == 'r')     
            nodes =ATMTreeView.findNodeById(parent).get_nodes();
      
        n.set_id('r'+newid);
        //alert(isOrganization);
        if (isOrganization) {
            n.set_imageUrl('icon_region.jpg');
            }
            
        else
        {
            n.set_imageUrl('icon_ATMRegionList.gif');
        }
        
        n.set_editingEnabled(editable); 
        nodes.add(n);
        
        ATMTreeView.render();        
        var newitem = new Array();
        newitem[0] = n.get_id();
        newitem[1] = n.get_text();
        
        ary.push(newitem);
        //ary[ary.length] = newitem;        
        if (isOrganization) {  
        ShowSuccess('Organization ' + regionName + ' was added.');
        }
        else
        {
        ShowSuccess('Region ' + regionName + ' was added.');
        }
        
    }       
     
    function onTreeContextMenu(sender, eventArgs)
    {
        var v= eventArgs.get_node().get_id();
      switch(v.charAt(0))
      {
        case 'r': 
          ContextMenuRegion.showContextMenu(eventArgs.get_event(), eventArgs.get_node()); 
          break; 

        case 'a': 
          //ContextMenuATM.showContextMenu(eventArgs.get_event(), eventArgs.get_node()); 
          break; 
 
        default: 
          alert('Error: selected node is neither ATM nor Region!'); 
          break; 
      }      
    }
    
    function Menu_onItemSelect(sender, eventArgs)
    {
      var menuItem = eventArgs.get_item();     
      var contextDataNode = menuItem.get_parentMenu().get_contextData(); 
      
      var feedback = '"' + menuItem.get_id() + '" command was issued on the "' + contextDataNode.get_text() + '" node from "' + contextDataNode.get_id() + '".'; 
      switch (menuItem.get_id())
      {
        case 'Report':
               parent.contentForm.location.href=  menuItem.get_value();
               ATMTreeView.selectNodeById(contextDataNode.get_id());
               break;
                       
        case 'region-delete': 
                if (contextDataNode.get_nodes().get_length() > 0) 
                    alert('Can not delete this region, delete child nodes first');
                else
                {
                    if (confirm('Do you want to delete '+contextDataNode.get_text()))
                        Callback_deleteNode.Callback(contextDataNode.get_id());
                }
				break;
       
        //        default: alert( feedback);                
      }
      return true; 
    }
    
    
	function RefreshAllFrames()
	{
		if (parent.contentForm.document.form1.refresh) 
		    parent.contentForm.document.form1.refresh.click();
	    
	}
    
    
     var previously_selected_id = new Array();
    function SelectionCompleted(id)
    {   
        while( previously_selected_id.length > 0 )
             ATMTreeView.findNodeById(previously_selected_id.pop()).set_marginImageUrl('');
                
        ATMTreeView.findNodeById(id).set_marginImageUrl('margin_arrow.gif');
        previously_selected_id.push(id);
        
        RefreshAllFrames();
        
//        if (contentPage!='')
//        {
//           }           
    }
    
    
    
    function MutilNodeSelectionCompleted(ids)
    {
        for (var i=0; i<ids.length; i++)
            ATMTreeView.findNodeById(ids[i]).set_marginImageUrl('margin_arrow.gif');
                
        previously_selected_id = ids;
        RefreshAllFrames();
    }

    var timer2 ;
    function NodeSelected(sender, arg) 
    {
        Callback_nodeSelected.Callback(arg.get_node().get_id());
        if (timer2)
             window.clearTimeout(timer2)
        timer2 = window.setTimeout("GetMutiSelection()",2000);
    }

    function GetMutiSelection()
    {
       var selectedNodes = ATMTreeView.get_multipleSelectedNodes();
       var ids='';
       
       for (var i=0; selectedNodes && i<selectedNodes.length; i++)
           ids += selectedNodes[i].get_id()+',';
         
       if (selectedNodes)
           Callback_multiNodeSelection.Callback(ids);    
    }
    function ShowError(msg)
    {
        document.getElementById('notificationArea').style.height = '20px';                    
        document.getElementById('notificationArea').innerHTML = '<img src="images/error.gif"/><span style="background-color:InfoBackground; vertical-align:top">'
            +msg+ '</span>';
       window.setTimeout('document.getElementById(\'notificationArea\').innerHTML="";document.getElementById(\'notificationArea\').style.height = \'2px\';',10000);
    }
    function ShowSuccess(msg)
    {        
        document.getElementById('notificationArea').style.height = '20px';  
        document.getElementById('notificationArea').innerHTML = '<img src="images/IconUpdate.gif"/><span style="color:green;font-weight:bold">'
            +msg+ '</span>';
       window.setTimeout('document.getElementById(\'notificationArea\').innerHTML=""; document.getElementById(\'notificationArea\').style.height = \'2px\';',5000);
    }
    function BeforeMove(sender, arg)
    { 
        if (arg.get_newParentNode().get_id().charAt(0)=='a')
        {  
           arg.set_cancel(true);    
           ShowError('An ATM can not contain other ATM or Region.');
        }
    }
    function NodeMoved(sender, arg) 
    {   Callback_Move.Callback(arg.get_node().get_id(), arg.get_newParentNode().get_id());  }  
    
    function NodeRenamed(sender, arg) 
    {   Callback_Rename.Callback(arg.get_node().get_id(), arg.get_node().get_text(),arg.get_oldText());  }    
    
    var selectionIndex=-1;
    function SearchTree(e, textbox)
    {        
      //try{
         var div = document.getElementById('searchDiv');
         var keyCode = window.event?event.keyCode:e.which;
         
         if (keyCode == 13)
         {
            var item = div.firstChild;
            for (var i=0; i<selectionIndex; i++)
                item = item.nextSibling
                        
            textbox.value = '';
            selectionIndex = -1;     
            MakeNodeSelection(item.attributes.getNamedItem('nodeId').value);
         }
         else if (keyCode == 27)
         {
            textbox.value ='';
            selectionIndex =-1;
         }
         else if (keyCode == 38)
         {
            
            if (selectionIndex == -1) 
                return;            
                
            var item = div.firstChild;
            for (var i=0; i<selectionIndex; i++)
                item = item.nextSibling;
            
            item.style.backgroundColor = 'InfoBackground';
                        
            selectionIndex--;
            if (selectionIndex > -1)
                item.previousSibling.style.backgroundColor = 'gray';
            return;
         }
         else if (keyCode == 40)
         {
            if (!div.firstChild) 
                return;
            var item = div.firstChild;
            for (var i=0; i<selectionIndex; i++)
                item = item.nextSibling;

            if ( selectionIndex == -1 )
            {
                item.style.backgroundColor = 'gray';
                selectionIndex++;
                return;
            }
            item.style.backgroundColor = 'InfoBackground';
            if (item.nextSibling)
            {
                item.nextSibling.style.backgroundColor = 'gray';
                selectionIndex++;
            }
            else
                item.style.backgroundColor = 'gray';
             return;
         }         
         
         if (textbox.value.length ==0)
         {
            div.style.display ='none';
            selectionIndex=-1;
         }
         else
         {
            var innerHTML ='';
            var input = textbox.value.toLowerCase();//.lower();            
            for (var i=0; i<ary.length; i++)
                if (ary[i][1].toLowerCase().indexOf(input) > -1 )
                    innerHTML += '<div nodeId="'+ary[i][0]+'"  onclick="MakeNodeSelection(\''+ary[i][0]+'\');">'+ary[i][1]+'</div>';
                    
            div.innerHTML = innerHTML;
            div.style.display = 'block';
         }
        // }catch(e){alert(e.message);}
       }
    function MakeNodeSelection(id)
    {
       document.getElementById('searchDiv').style.display='none';
       document.getElementById('searchDiv').innerHTML ='';
       ATMTreeView.selectNodeById(id);
    }
         
    function SelectNode(id)
    {   ATMTreeView.selectNodeById(id); }


ctrlPressed = false;
function checkForCtrlKey(e)
{
    var kc = window.event?event.keyCode:e.which;
    if (kc == 17) 
	    ctrlPressed = true;
}
function checkForCharKey(e)
{
    //    y -- 89 // splitter
  //  u -- 85 // for home page

   var kc = window.event?event.keyCode:e.which;
    if (kc == 85 && ctrlPressed ) 
    {	
        if (window.location.toString().indexOf('Main') > 0) 
           document.getElementById('contentFrame').src = 'Home.aspx';            
        else
            window.location='../Home.aspx';
    }
    
//Check for q key and collapsed
    else if (kc == 89 && ctrlPressed ) 
    {
        hideSplitter();
    }
// if the ctrl key is up then set it to false;
        if (kc == 17) 
        ctrlPressed =false;

}

function hideSplitter()
{
     
     if (Splitter1.get_panes()[0].get_collapsed() == null || Splitter1.get_panes()[0].get_collapsed() == false)  
            Splitter1.get_panes()[0].collapse();
        else
            Splitter1.get_panes()[0].expand();
     
    
}