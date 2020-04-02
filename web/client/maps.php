<?php
require_once('config.php');
require_once('auth.inc.php');


if ( isset($_GET['action']) )
{
	if ($_GET['action'] == 'delete' && isset($_GET['file']) )
	{
		$file = $_GET['file'];
		$file = str_replace('..', '', $file); // exclude injection
		
		// protect from parent directories which are not allowed
		$found = false;
		foreach (Config::$MapDirs as $d)
		{
			if ( starts_with($file, '/' . $d) )
			{
				$found = true;
			}
		}

		$file_path = Config::$MapPath . $file;
		if ( $found && file_exists($file_path) )
		{
			unlink($file_path);
			echo "File <b>$file</b> was deleted";
		}
		else
			echo "File <b>$file</b> does not exist!";
		
	}
	
	if ($_GET['action'] == 'move' && isset($_GET['src']) && isset($_GET['dst']) )
	{
		$dst = $_GET['dst'];
		
		if ( !in_array($dst, Config::$MapDirs) )
		{
			echo "Not allowed path /" . $dst;
		}
		else
		{
			$file = $_GET['src'];
			$file = str_replace('..', '', $file); // exclude injection
			
			$file_path = Config::$MapPath . $file;
			$dst_path = Config::$MapPath . '/' . $dst  . '/';
			if ( file_exists($file_path) )
			{
				rename($file_path, $dst_path . pathinfo($file_path, PATHINFO_BASENAME));
				echo "File <b>$file</b> was moved to /" . $dst;
			}
			else
				echo "File <b>$file</b> does not exist!";
			
		}
	}
	
	echo "<br><br>&larr; <a href='maps.php'>Back to maps list</a>";
	die();
}


function read_dir($path)
{
	foreach (new DirectoryIterator($path) as $f)
	{
		if ( $f->isDot() )
			continue;
		if ( $f->getFilename() == '.htaccess' )
			continue;
		
		$full_path = $path . '/' . $f->getFilename();
		if( $f->isDir() )
		{
			read_dir($full_path);
			continue;
		}
		$print_path =  str_replace(Config::$MapPath, '', $full_path);
		
		echo '<tr>
			<td style="padding-right: 10px">' . $print_path . '</td>

			<td style="padding-right: 10px">
				<select>
' . print_options($print_path) . '
				</select>
				<button onclick="return move(this)" data-path="' . $print_path . '">move</button>
			</td>
			
			<td style="padding-right: 10px">
				<button style="color: crimson" onclick="return remove(this)" data-path="' . $print_path . '">delete</button>
			</td>
		</tr>';
		
	}
}

function print_options($path)
{
	$html = '';
	foreach (Config::$MapDirs  as $d)
	{
		if ( starts_with($path, '/' . $d) )
			continue;
		$html .= '<option value="' . $d . '">/' . $d . '</option>';	
	}
	return $html;
}

function starts_with($string, $startString) 
{ 
    $len = strlen($startString); 
    return (substr($string, 0, $len) === $startString); 
} 

?>
&larr; <a href='/servers/'>Back to servers</a>
<h2>NFK Server Maps</h2>

<p>
	<a href="//<?php echo $_SERVER['HTTP_HOST'] ?>/maps"><?php echo $_SERVER['HTTP_HOST'] ?>/maps</a>
</p>

<table>
<tr style="background: silver;">
	<th>file name</th>
	<th colspan="2">action</th>
</tr>
<tbody>
<?php
read_dir(Config::$MapPath);
?>
</tbody>
</table>


<script language="javascript">
	function move(el)
	{
		var path = el.getAttribute('data-path');
		var el2 = el.parentElement.querySelector('select');
		var val = el2.options[el2.selectedIndex].value;
		if ( window.confirm('Are you sure want to perform move?\n' + path + ' -> /' + val ) ) 
		{
			window.location.href = '?action=move&src=' + path + '&dst=' + val;
			return true;
		}
		return false;
	}
	
	function remove(el)
	{
		var path = el.getAttribute('data-path');
		if ( window.confirm('Are you sure want to completely remove this map?\n' + path) )
		{
			window.location.href = '?action=delete&file=' + path;
			return true;
		}
		return false;
	}
</script>
