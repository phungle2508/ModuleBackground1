namespace ModuleBackground;

public class CInfoObjCut
{
	private string m_InfoText;

	private string m_Image;

	public string InfoText
	{
		get
		{
			return m_InfoText;
		}
		set
		{
			m_InfoText = value;
		}
	}

	public string ImageName
	{
		get
		{
			return m_Image;
		}
		set
		{
			m_Image = value;
		}
	}

	public CInfoObjCut(string strInfoText, string strImage)
	{
		m_InfoText = strInfoText;
		m_Image = strImage;
	}
}
