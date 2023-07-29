using UnityEngine;

public class PlinkoTeleporter : MonoBehaviour
{
    public GameObject portalIn;
    public GameObject portalOut;
    public AudioClip[] teleportSound;
    public ParticleSystem teleportParticles;

    //TODO good place for upgrades. Score + on teleport?
    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.CompareTag("PlinkoBall"))
        {
            //adjust position to be in the same releative position to the other portal
            Vector3 pos = col.transform.position;
            var outPos = portalOut.transform.position;
            pos.x = outPos.x + (pos.x - portalIn.transform.position.x);
            pos.y = outPos.y;
            col.transform.position = pos;
            if (!GameMaster.instance.ModeMaster.IsMode(ModeMaster.Gamemode.Hivemind)) return;
            SFXMaster.instance.PlayOneShot(teleportSound[UnityEngine.Random.Range(0, teleportSound.Length)]);
            teleportParticles.Play();
        }
    }
}