ctrlPressed = false;
var keyCode = 0;
function checkForCtrlKey(e)
{
    var kc = window.event?event.keyCode:e.which;
    if (kc == 17) 
	    ctrlPressed = true;
}
function checkForSQLInjection() {
    
      var kc = window.event?event.keyCode:e.which;
      
    if (kc==222)
        return false;
}
function checkForCharKey(e)
{
//    y -- 89 // splitter
  //  u -- 85 // for home page

    
    if (e.keyCode == 85 && ctrlPressed ) 
    {	
        if (window.location.toString().indexOf('Main') > 0) 
           document.getElementById('contentFrame').src = 'Home.aspx';            
        else
            window.location='../Home.aspx';
    }
    else if (e.keyCode == 89 && ctrlPressed ) 
    {
      keyCode = 89;
      parent.top.focus();
      //alert(parent.top.document.getElementById('Splitter1').innerHTML);
      
      
    }
     
    if (e.keyCode == 17) 
       ctrlPressed =false;
}


    function ShowSuccess(msg)
    {   
        document.getElementById('notificationArea').style.height = '20px';             
        document.getElementById('notificationArea').innerHTML = '<img src="../images/IconUpdate.gif"/><span style="color:red; font-weight:bold">'
            +msg+ '</span>';
       window.setTimeout('document.getElementById(\'notificationArea\').innerHTML=""; document.getElementById(\'notificationArea\').style.height = \'2px\';',5000);
    }

    function setCheck(val, excludeId)
	{
		    var inputs = document.getElementsByTagName('input');
		    for (var i = 0; i < inputs.length; i++)
            {
                var typ = inputs[i].type;
                var id = inputs[i].id;
                if (typ=="checkbox" && id != excludeId) {
                        if (!(inputs[i].disabled))
                        inputs[i].checked = val;    
                        }
                
            }
            }

            function SetCheckBoxStatus(val, includeId) {
                //alert('d');
                var inputs = document.getElementsByTagName('input');
                for (var i = 0; i < inputs.length; i++) {
                    var typ = inputs[i].type;
                    var id = inputs[i].id;
                    //alert(includeId);
                    if (typ == "checkbox") {
                       // alert(id);
                        if (id.indexOf(includeId) >= 0) {

                            if (!(inputs[i].disabled)) {
                                inputs[i].checked = val;
                            }
                        }

                    }
                }

            }

	function setCheckATMUsers(val,name)
	{
	   
		    var inputs = document.getElementsByTagName('input');
		    for (var i = 0; i < inputs.length; i++)
            {
                var typ = inputs[i].type;
                
                if (typ=="checkbox" ) {
                        if (!(inputs[i].disabled) && inputs[i].name.indexOf(name)!=-1)
                        {
                        inputs[i].checked = val;    
                        }
                        }
                
            }
	}

	function hideProgressBar() {
	    parent.document.getElementById('divWait').style.visibility = 'hidden';
	}
	

function SetTabIndex(id, index, focus)
		{
			var object = document.getElementById(id);
			
			object.tabEnabled = true;
			object.tabIndex = index;
			
			if (focus)
				object.focus();
		}
            
var f = function (e) {
    window.removeEventListener("message", f, false);
}
window.addEventListener("message", f);

function receiveMessage(event) {
    if (event.origin.indexOf('localhost') === -1) {
        return;
    }
}

window.addEventListener("message", receiveMessage, false);