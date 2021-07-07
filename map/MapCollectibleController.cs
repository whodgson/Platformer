using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapCollectibleController : MonoBehaviour
{
    SphereCollider i_trigger;
    AudioSource i_audio;

    // Start is called before the first frame update
    void Start()
    {
        i_trigger = GetComponent<SphereCollider>();
        i_audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Player")
        {
            AudioSource.PlayClipAtPoint(i_audio.clip, this.transform.position);
            Destroy(this.gameObject);
            Debug.Log("EGG");
        }
    }


}
