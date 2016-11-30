/****************************************************************************
 *  This file is available under Creative Commons Attribution Share Alike.  *
 *  You are free to:														*
 *  Share — copy and redistribute the material in any medium or format		*
 *  Adapt — remix, transform, and build upon the material					*
 *  	for any purpose, even commercially.									*
 *																			*
 *  Under the following terms:												*
 *  Attribution — You must give appropriate credit, provide a link to the 	*
 *		license, and indicate if changes were made. You may do so in any 	*
 *		reasonable manner, but not in any way that suggests the licensor 	*
 *		endorses you or your use.											*
 *  ShareAlike — If you remix, transform, or build upon the material, you 	*
 *		must distribute your contributions under the same license as the 	*
 *		original.															*
 *                                                                          *
 *   Source: <http://wiki.unity3d.com/index.php?title=DrawArrow>		    *
 *   License: >http://creativecommons.org/licenses/by-sa/3.0/>  			*
 ****************************************************************************/

using UnityEngine;
using System.Collections;

public static class DrawArrow
{
	public static void ForGizmo(Vector3 pos, Vector3 direction, float arrowHeadLength = 2f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.DrawRay(pos, direction);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}
	
	public static void ForGizmo(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 2f, float arrowHeadAngle = 20.0f)
	{
		Gizmos.color = color;
		Gizmos.DrawRay(pos, direction);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Gizmos.DrawRay(pos + direction, right * arrowHeadLength);
		Gizmos.DrawRay(pos + direction, left * arrowHeadLength);
	}
	
	public static void ForDebug(Vector3 pos, Vector3 direction, float arrowHeadLength = 2f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength);
		Debug.DrawRay(pos + direction, left * arrowHeadLength);
	}
	public static void ForDebug(Vector3 pos, Vector3 direction, Color color, float arrowHeadLength = 2f, float arrowHeadAngle = 20.0f)
	{
		Debug.DrawRay(pos, direction, color);
		
		Vector3 right = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180+arrowHeadAngle,0) * new Vector3(0,0,1);
		Vector3 left = Quaternion.LookRotation(direction) * Quaternion.Euler(0,180-arrowHeadAngle,0) * new Vector3(0,0,1);
		Debug.DrawRay(pos + direction, right * arrowHeadLength, color);
		Debug.DrawRay(pos + direction, left * arrowHeadLength, color);
	}
}