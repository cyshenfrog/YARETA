using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadingTest : MonoBehaviour
{
    // Start is called before the first frame update
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1);
        Debug.Log("LoadingTest");
        SceneManager.LoadSceneAsync(0, LoadSceneMode.Additive);
    }

    // Update is called once per frame
    private void Update()
    {
    }
}