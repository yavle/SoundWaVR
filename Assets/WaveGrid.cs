using UnityEngine;
using System.Collections;

public class WaveGrid : MonoBehaviour {

	//public GameObject plane;

	public bool showMain = true;
	public bool showSub = true;

	public int gridSizeX = 30;
	public int gridSizeY = 0;
	public int gridSizeZ = 30;

	public float smallStep = 0.2f;
	public float gridStep = 0.2f;

	public float startX = -20f;
	public float startY = 0f;
	public float startZ = -20f;

	public float soundEventStartTime = 0f;
	public float soundEventX = 0.5f;
	public float soundEventY = -3f;
	public float soundEventZ = 20f;

	public float soundEventWaveFq = 440f; // in Hz
	public float realSoundWaveSpeed	= 315f; // in m/s
	public float realSoundWaveSpeedScaleFactor = 0.01f; //less means slower

	public float maxWaveAmp = 1f;
	public float dSAttenuationFactor = 0.3f;

	private Material lineMaterial;

	public  Color mainColor = new Color(1f, 1f, 1f, 0.1f);
	public Color subColor = new Color(0f, 0.5f, 0f, 0.3f);

    private SoundFieldManager thinAir;
    public float soundEventAhead;

	public AudioClip soundFx;

	private AudioSource source;
	


    void Start ()
    {
		GameObject go = GameObject.Find ("SoundManagerHolder");
		thinAir = go.GetComponent <SoundFieldManager> ();
        // line = transform.GetComponent<LineRenderer> ();
        // line.SetVertexCount (5);
        // line.SetPosition (0, new Vector3 (-1, 1, 0));
        // line.SetPosition (1, new Vector3 (1, 1, 0));
        // line.SetPosition (2, new Vector3 (1, -1, 0));
        // line.SetPosition (3, new Vector3 (-1, -1, 0));
        // line.SetPosition (4, new Vector3 (-1, 1, 0));
        source = GetComponent<AudioSource>();
		soundEventAhead = 1;
    }



	void CreateLineMaterial()
	{
		if (!lineMaterial)
		{
			// Unity has a built-in shader that is useful for drawing
			// simple colored things.
			var shader = Shader.Find("Hidden/Internal-Colored");
			lineMaterial = new Material(shader);
			lineMaterial.hideFlags = HideFlags.HideAndDontSave;
			// Turn on alpha blending
			lineMaterial.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
			lineMaterial.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
			// Turn backface culling off
			lineMaterial.SetInt("_Cull", (int)UnityEngine.Rendering.CullMode.Off);
			// Turn off depth writes
			lineMaterial.SetInt("_ZWrite", 0);
		}
	}

	void OnPostRender()
	{

		
		if (thinAir.soundEventStartTime == 0) {
			return;
		}
		//new soundPLayed
		if (soundEventStartTime != thinAir.soundEventStartTime) {
			soundEventAhead = 1;
		}
		soundEventStartTime = thinAir.soundEventStartTime;
		soundEventX = startX = thinAir.soundEventPosition.x;
		soundEventZ = startZ = thinAir.soundEventPosition.z;
		soundEventY = thinAir.soundEventPosition.y;

// print("helloz" + startX);
		CreateLineMaterial();
		// set the current material
		lineMaterial.SetPass(0);

		GL.Begin(GL.LINES);


		float centerX = startX + (gridSizeX - startX) / 2;
		float centerZ = startZ + (gridSizeZ - startZ) / 2;

		float soundEventTime = Time.time - soundEventStartTime;
		float soundWaveSpeed = realSoundWaveSpeed * realSoundWaveSpeedScaleFactor;
		float soundEventHorizon = soundEventTime * soundWaveSpeed;

		// transform.localPosition = new Vector3(0, 0, 0);
		// print(transform.position);


//		transform.lo

			

		
		if (Mathf.Sign (Mathf.Sqrt (Mathf.Pow ((transform.position.x - soundEventX), 2) + Mathf.Pow ((transform.position.z - soundEventZ), 2)) - soundEventHorizon) != soundEventAhead) {
			soundEventAhead = Mathf.Sign (Mathf.Sqrt (Mathf.Pow ((transform.position.x - soundEventX), 2) + Mathf.Pow ((transform.position.z - soundEventZ), 2)) - soundEventHorizon);
			
			source.PlayOneShot(soundFx,volumeByDtAndDS (soundEventTime, Mathf.Sqrt (Mathf.Pow ((transform.position.x - soundEventX), 2) + Mathf.Pow ((transform.position.z - soundEventZ), 2))));

		}
		if (soundEventAhead < 0) {
			// source.volume = volumeByDtAndDS (soundEventTime, Mathf.Sqrt (Mathf.Pow ((transform.position.x - soundEventX), 2) + Mathf.Pow ((transform.position.z - soundEventZ), 2)));
		}
		for (int i = 0; (i * gridStep) <= soundEventHorizon; i += 1) {

			float vol = volumeByDtAndDS(soundEventTime - i * gridStep / soundWaveSpeed, soundEventHorizon);
			GL.Color(new Color(1f, 1f, 1f, Mathf.Min(Mathf.Abs((i * gridStep) - soundEventHorizon )/3, vol)));
			if (vol > 0.01f) {
				for (int dXn = -i; dXn <= i; dXn += 1) {
					float dX = dXn * gridStep;
					for (int dZn = -i; dZn <= i; dZn += 1) {
						float dZ = dZn * gridStep;
						float curX = soundEventX + dX;
						float curZ = soundEventZ + dZ;
						var waveLengthFactor =  2 * Mathf.PI * realSoundWaveSpeed / soundEventWaveFq;
						var waveAmpFactor = - maxWaveAmp * vol;
						if ((Mathf.Abs(dXn) == i || Mathf.Abs(dZn) == i) && Mathf.Sqrt(Mathf.Pow(dX,2) + Mathf.Pow(dZ + gridStep,2)) <= soundEventHorizon) {
							GL.Vertex3(curX, soundEventY + waveAmpFactor * Mathf.Cos((Mathf.Sqrt(Mathf.Pow(dX,2) + Mathf.Pow(dZ,2)) - soundEventHorizon) * waveLengthFactor)  , curZ);
							GL.Vertex3(curX, soundEventY + waveAmpFactor * Mathf.Cos((Mathf.Sqrt(Mathf.Pow(dX,2) + Mathf.Pow(dZ + gridStep,2)) - soundEventHorizon) * waveLengthFactor)  , curZ + gridStep);

							GL.Vertex3(curX, soundEventY + waveAmpFactor * Mathf.Cos((Mathf.Sqrt(Mathf.Pow(dX,2) + Mathf.Pow(dZ,2)) - soundEventHorizon) * waveLengthFactor)  , curZ);
							GL.Vertex3(curX + gridStep, soundEventY + waveAmpFactor * Mathf.Cos((Mathf.Sqrt(Mathf.Pow(dX + gridStep,2) + Mathf.Pow(dZ,2)) - soundEventHorizon) * waveLengthFactor) , curZ);
						}

					}
				}
			}
		}




		GL.End();
	}

	// simple linear dependencies aren't physically correct of course 
	float volumeByDtAndDS(float dT, float DS)
	{
		float realVolume =  ((dT >= 0) && (dT <= 2)) ? (2 - dT) / 2 : 0;
		float travelledVolume = (DS < 7) ? (realVolume / Mathf.Pow((DS * dSAttenuationFactor + 1), 2)) : (realVolume / Mathf.Pow((DS * dSAttenuationFactor + 1), 3)) ; 
		return travelledVolume;
	}
}

// Принципы:
//Танцуем от длины волны.
//Показываем ту длину волны, которая звучит - базовую частоту. 
//Для сравнительного восприятия - объекты эталонных размеров:  рост наблюдателя, деревья-кусты,
//Показываем синусоиду, модулированную громкостью сигнала


// backlog:
// [V] soundEventStartTime
// [V] soundEventStartPosition (X,Y,Z)
// [V] iterate through dots by squares around soundStartPosition - untilReach soundEventHorizon ;)
// [V] realSoundWaveSpeedScaleFactor, realSoundWaveSpeed - wave "movement" mechanics 
// [V] soundEventWaveLength = soundEventWaveFq * realSoundWaveSpeed, realLengthScaleFactor, waveHight 
// [V] dSAttenuationFactor - attenuation with distance mechanics
// [V] attenuation with time mechanics
// [V] show only affected lines (and pivots) 
//     [ ] (dont show lines after attenuation)
// [V] Make "edges" of sound wave transparent - better visual of "wave appearig out of thin air"
// [V] play wave "from instrument" - from its position
//   [ ] with its FQ 
//	 [ ] and soundAsset
//   [V] and WHEN it activates sound
// [V] basic scene (ground, 1 instrument)
// [ ] modulate signal with sound volume
// [V-] sound output mechanics
//     [V] play real sound
// 	   [V] at the moment SoundEvenHorizon reach the Observer
//     [ ] Stereo output - pan depends on head orientation
//     [ ] Ear-to-ear latency  


// [V] make the Wave Birth look like falling ball continuation
// [ ] mvp scene 
	// [ ] several instruments
	// [ ] travel between observing points 
	// [ ] text hints
// [ ] extended scene - cliff, trees, rocks, skies, clouds 