using System.Collections;
using UnityEngine;

public class Cannon : MonoBehaviour {

    Vector3 origin;
    Vector3 target;
    Vector3 apex;
    [SerializeField] private float rotateSpeed = 30;

    [SerializeField] private AudioClip shotSound;
    [SerializeField] private AudioClip music;



	[SerializeField] private float animationSpeedup = 1f;
	[SerializeField] private AnimationCurve pathSpeedMultiplier;

    float aimAngle;
    float rotateAngle;
	[SerializeField] public float speed;

	[SerializeField] bool skipAnimations = false;
	bool firing;
    bool clockwise;
	bool inoperable = false;

    Vector3 cross;

    float timeToWait;

	CannonPath path;
	Vector3[] pathNodes;

	public GameObject cinematicCamera;
	public GameObject mouthCamera;

	// Use this for initialization
	void Start () {
		path = transform.Find("CannonPath").GetComponent<CannonPath>();
		if (null == path) {
			inoperable = true;
			return;
		}

		pathNodes = path.pathNodes;
		if (null == pathNodes) {
			inoperable = true;
			return;
		}
		inoperable = false;

		firing = false;
        clockwise = true;
		origin = path.start.position;
		target = path.end.position;

		Vector3 shotVector = pathNodes[1] - pathNodes[0];

		aimAngle = 90f - Vector3.Angle(shotVector, Vector3.ProjectOnPlane(shotVector, Vector3.up));

        rotateAngle = Vector3.Angle(new Vector3(transform.forward.x, 0f, transform.forward.z),
                                    new Vector3(target.x, 0f, target.z) - new Vector3(origin.x, 0f, origin.z));

        cross = Vector3.Cross(new Vector3(transform.forward.x, 0f, transform.forward.z),
                              new Vector3(target.x, 0f, target.z) - new Vector3(origin.x, 0f, origin.z));

        if (cross.y < 0f) {
            clockwise = false;
        }

        //print ("Aim angle: " + aimAngle + "\tRotate angle: " + rotateAngle);

        if (null == GetComponent<AudioSource> ()) {
            gameObject.AddComponent<AudioSource> ();
        }

        timeToWait = -1f;

		if (null != mouthCamera) {
			mouthCamera.transform.SetParent(transform.Find("Barrel"));
		}
	}

    public void Fire (Rigidbody projectile) {
        if (false == inoperable && false == firing) {

			if (null != cinematicCamera) {
				cinematicCamera.SetActive(true);
			}

			if (skipAnimations) {
				//print("SUCK ME OFF");
				firing = true;
				projectile.gameObject.AddComponent<FollowPath>();
				projectile.GetComponent<FollowPath>().SetPath(pathNodes, speed, true);
			} else {
				StartCoroutine(Fire(aimAngle, rotateAngle, speed, projectile));
			}
        }
    }

    IEnumerator Fire (float aimAngle, float rotateAngle, float velocity, Rigidbody projectile) {
        firing = true;

        Transform barrel = transform.Find("Barrel");

        yield return new WaitForSeconds (3f / animationSpeedup);

        if (null != music) {
            GetComponent<AudioSource> ().clip = music;
            GetComponent<AudioSource> ().Play ();
            timeToWait = music.length;
        }

        float timeElapsed = 0f;

        //print ("Clockwise: " + clockwise);

        float angleTraversed = 0;

        //print("Rotating " + rotateAngle);
        while (angleTraversed < rotateAngle) {
            transform.Rotate(transform.up, (clockwise ? 1 : -1) * rotateSpeed * Time.deltaTime);
            angleTraversed += rotateSpeed * Time.deltaTime;

            projectile.position = barrel.position;
            projectile.velocity = Vector3.zero;

            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }

        angleTraversed = 0;

        yield return new WaitForSeconds(3f / animationSpeedup);
        timeElapsed += 3f;


        //print("Aiming " + aimAngle);
        while (angleTraversed < aimAngle) {
            //barrel.Rotate(barrel.transform.right, rotateSpeed * Time.deltaTime);
            barrel.localEulerAngles += Vector3.right * rotateSpeed * Time.deltaTime;
            angleTraversed += rotateSpeed * Time.deltaTime;

            projectile.position = barrel.position;
            projectile.velocity = Vector3.zero;

            yield return new WaitForEndOfFrame();
            timeElapsed += Time.deltaTime;
        }

		yield return new WaitForSeconds(Mathf.Max(5f, timeToWait - timeElapsed - 2f));

		if (null != cinematicCamera) {
			cinematicCamera.SetActive(false);
		}

		if (null != mouthCamera) {
			mouthCamera.SetActive(true);
		}

		yield return new WaitForSeconds(2f);

		GetComponentInChildren<MouthController>().SetExpression(MouthController.MouthState.smile);

		yield return new WaitForSeconds(2f / animationSpeedup);


		if (null != mouthCamera) {
			mouthCamera.SetActive(false);
		}
		//print("BOOM! " + velocity + " MPS");

        if (null != shotSound) {
            GetComponent<AudioSource> ().clip = shotSound;
            GetComponent<AudioSource> ().Play ();
        }

		//projectile.position = barrel.position;
		//projectile.velocity = Vector3.zero;
		//projectile.useGravity = false;
		projectile.gameObject.AddComponent<FollowPath>();
		projectile.GetComponent<FollowPath>().SetPath(pathNodes, velocity, true, pathSpeedMultiplier);


        angleTraversed = 0;
        yield return new WaitForSeconds(3f / animationSpeedup);



		//print("Resetting aim " + aimAngle);
        while (angleTraversed < aimAngle) {
            //barrel.Rotate(barrel.transform.right, -rotateSpeed * Time.deltaTime);
            barrel.localEulerAngles += Vector3.right * -rotateSpeed * Time.deltaTime;
            angleTraversed += rotateSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        angleTraversed = 0;
        yield return new WaitForSeconds(3f / animationSpeedup);

        //print("Resetting angle " + rotateAngle);
        while (angleTraversed < rotateAngle) {
            transform.Rotate(transform.up, (clockwise ? -1 : 1) * rotateSpeed  * Time.deltaTime);
            angleTraversed += rotateSpeed * Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }

        yield return new WaitForSeconds(3f / animationSpeedup);

        gameObject.GetComponentInChildren<EnterCannon> ().Reset ();
        firing = false;
    }

    //private void OnDrawGizmos () {
    //    Gizmos.color = Color.red;
    //    Transform barrel = transform.Find("Barrel");
    //    Gizmos.DrawRay(barrel.position, barrel.forward * velocity);
    //    Gizmos.DrawSphere(apex, 10f);
    //    Gizmos.DrawSphere(target, 3f);

    //    //Gizmos.color = Color.green;
    //    //Gizmos.DrawRay(transform.position, transform.position + barrel.forward * 50);
    //}
}
