using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UAP_Video_MainMenu : MonoBehaviour
{
	public GameObject m_SettingsMenu = null;

	//////////////////////////////////////////////////////////////////////////

	public void OnSettingsButtonPressed()
	{
		Instantiate(m_SettingsMenu);
	}
}
