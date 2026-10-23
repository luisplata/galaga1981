using UnityEngine;

namespace V2
{
    /// <summary>
    /// Fondo de estrellas (juice M3): exactamente 2 SpriteRenderer en drawMode Tiled del
    /// sprite 10_Fondo, scroll vertical con wrap por tile. La capa tiled se dimensiona al
    /// viewport + 2 tiles de margen y el offset se envuelve en [0, tileH), así la cobertura
    /// del viewport es total en cualquier offset. Sin partículas ni fades fullscreen.
    /// </summary>
    public class Starfield : MonoBehaviour
    {
        [SerializeField] private Camera gameCamera;
        [SerializeField] private SpriteRenderer slowLayer;
        [SerializeField] private SpriteRenderer fastLayer;
        [SerializeField] private Sprite starSprite;
        [SerializeField] private float slowSpeed = 0.15f;
        [SerializeField] private float fastSpeed = 0.35f;
        [SerializeField] private float slowAlpha = 0.5f;
        [SerializeField] private float fastAlpha = 0.25f;

        private float slowOffset;
        private float fastOffset;
        private float tileWidth = 1f;
        private float tileHeight = 1f;
        private bool configured;

        private void Awake()
        {
            ConfigureLayers();
        }

        private void ConfigureLayers()
        {
            if (configured) return;
            if (gameCamera == null) gameCamera = Camera.main;
            Configure(slowLayer, -10, slowAlpha);
            Configure(fastLayer, -9, fastAlpha);
            configured = true;
        }

        private void Configure(SpriteRenderer sr, int sortingOrder, float alpha)
        {
            if (sr == null) return;

            if (starSprite != null) sr.sprite = starSprite;
            if (sr.sprite != null)
            {
                tileWidth = sr.sprite.bounds.size.x;
                tileHeight = sr.sprite.bounds.size.y;
            }

            sr.drawMode = SpriteDrawMode.Tiled;
            sr.tileMode = SpriteTileMode.Continuous;
            sr.sortingOrder = sortingOrder;
            sr.color = new Color(1f, 1f, 1f, alpha);
        }

        private void LateUpdate()
        {
            ConfigureLayers();
            if (gameCamera == null || slowLayer == null || fastLayer == null) return;

            slowOffset = Mathf.Repeat(slowOffset + slowSpeed * Time.deltaTime, tileHeight);
            fastOffset = Mathf.Repeat(fastOffset + fastSpeed * Time.deltaTime, tileHeight);

            Layout(slowLayer, slowOffset);
            Layout(fastLayer, fastOffset);
        }

        private void Layout(SpriteRenderer sr, float offset)
        {
            float halfHeight = gameCamera.orthographicSize;
            float halfWidth = halfHeight * gameCamera.aspect;
            Vector3 cameraPosition = gameCamera.transform.position;

            sr.size = new Vector2(
                halfWidth * 2f + tileWidth * 2f,
                halfHeight * 2f + tileHeight * 2f);

            // Punto de partida centrado en la cámara; luego se corrige con bounds para que el
            // área tiled cubra [cámara - half - tile, cámara + half + tile] sea cual sea el
            // pivot del sprite importado (10_Fondo viene con alignment Center).
            sr.transform.position = new Vector3(cameraPosition.x, cameraPosition.y - offset, 0f);

            Bounds bounds = sr.bounds;
            float deltaX = (cameraPosition.x - halfWidth - tileWidth) - bounds.min.x;
            float deltaY = (cameraPosition.y - halfHeight - tileHeight - offset) - bounds.min.y;
            Vector3 position = sr.transform.position;
            sr.transform.position = new Vector3(position.x + deltaX, position.y + deltaY, 0f);
        }
    }
}
