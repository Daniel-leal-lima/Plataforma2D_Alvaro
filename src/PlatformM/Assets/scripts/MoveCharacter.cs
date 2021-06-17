using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;               // Todas as bibliotecas necessárias para trabalhar com UI
using UnityEngine.SceneManagement;  // Todas as bibliotecas necessárias para trabalhar com o gerenciamento de cenas

public class MoveCharacter : MonoBehaviour {

    Rigidbody2D character;          // Para poder movimentar o personagem (física)
    public float vel;               // Velocidade do personagem
    public float jumpVel;           // Velocidade do pulo
    bool jump;                      // Saber se está pulando ou não
    int stars;                      // Quantidade de estrelas
    int lifes;                      // Quantidade de vidas
    public Text lifesTxt;           // Texto que poderá ser editado na interface da Unity (public)
    public Text starsTxt;           // Texto que poderá ser editado na interface da Unity (public)
    public Text gameOverTxt;        // Texto de Game Over
    public Text winTxt;             // Texto de Vitória
    public Button start;            // Botão para iniciar o jogo
    public Button restart;          // Botão para reiniciar o jogo
    public Animator heroAnim;       // Componente Animator (responsável por controlar as animações)
    public GameObject door;         // GameObject porta
    public AudioSource gameAudio;   // Componente AudioSource (responsável por controlar uma fonte de áudio)
    public AudioClip[] sounds;      // Array (container) de audio que irá selecionar qual som irá tocar em qual momento
 

	void Start () {
        Time.timeScale = 0;                                 // Jogo "pausado" ao iniciar (o tempo não está sendo atualizado)
        character = gameObject.GetComponent<Rigidbody2D>(); // Rigidbody do GameObject associado ao script
        stars = 0;                                          // Quantidade inicial de estrelas                                      
        lifes = 3;                                          // Quantidade inicial de vidas
        lifesTxt.text = lifes.ToString();                   // Propriedade texto do lifesTxt é igual à quantidade de vidas convertida em String (conjunto de caracteres)
        starsTxt.text = stars.ToString();                   // Propriedade texto do starsTxt é igual à quantidade de estrelas convertida em String (conjunto de caracteres)
        start.gameObject.SetActive(true);                   // Botão de Start ativo ao iniciar o jogo
        restart.gameObject.SetActive(false);                // Botão de Restart inativo ao iniciar o jogo
        gameOverTxt.enabled = false;                        // Texto de Game Over inativo ao iniciar o jogo
        winTxt.enabled = false;                             // Texto de Vitória inativo ao iniciar o jogo
        heroAnim.SetBool("Jump", false);         // Estado de animação Jump é falso ao iniciar o jogo
        heroAnim.SetBool("Walk", false);         // Estado de animação Walk é falso ao iniciar o jogo
        heroAnim.SetBool("Run", false);          // Estado de animação Run é falso ao iniciar o jogo

	}
	
	
	void Update () {
        if(Input.GetKey(KeyCode.RightArrow))                                           // Se a tecla pressionada for a seta para a direita...
        {
            character.transform.Translate(vel * Time.deltaTime, 0, 0);          // anda para a direita de acordo com o valor da velocidade
            heroAnim.SetBool("Walk", true);                                 // Chama o estado de animação Walk (true)
            if(this.gameObject.transform.localScale.x < 0)                             // S e a escala do personagem é menor que zero
            {                     
                this.gameObject.transform.localScale = new Vector3((this.gameObject.transform.localScale.x * -1), this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z); // inverte o X
            }

        }
        if (Input.GetKey(KeyCode.LeftArrow))                                          // Se a tecla pressionada for a seta para a esquerda...
        {
            character.transform.Translate(-vel * Time.deltaTime, 0, 0);        // anda para a direita de acordo com o valor da velocidade
            heroAnim.SetBool("Walk", true);                                // Chama o estado de animação Walk (true)
            if (this.gameObject.transform.localScale.x > 0)                           // Se a escala do personagem é maior que zero
            {
                this.gameObject.transform.localScale = new Vector3((this.gameObject.transform.localScale.x * -1), this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z); // inverte o x
            }

        }
        if (Input.GetKeyDown(KeyCode.Space) && jump)                            // Se a tecla pressionada (uma única vez) for a barra de espaço E jump for verdadeiro (&& jump)
        {
            character.AddForce(Vector2.up * jumpVel, ForceMode2D.Impulse);      // Personagem sofre uma força (AddForce) para cima (Vector2.up) com velocidade jumpVel e do tipo IMPULSO (impulse)
            heroAnim.SetBool("Jump", true);                          // Chama o estado de animação Jump (true)

        }
        if (Input.GetKey(KeyCode.W))                                            // Se a tecla pressionada for W...
        {
            vel = 25;                                                           // Altera o valor da velocidade para 25
            heroAnim.SetBool("Run", true);                           // Chama o estado de animação Run (true)

        }
       
        if (Input.GetKeyUp(KeyCode.W))                                          // Ao SOLTAR a tecla W...
        {
            vel = 15;                                                           // Altera o valor da velocidade para 15
            heroAnim.SetBool("Run", false);                          // Chama o estado de animação Run (true)

        }

	}

    private void OnCollisionEnter2D(Collision2D collision)                    // QUANDO ACONTECER UMA COLISÃO 2D (um objeto 2D encostar com outro)
    {
        if(collision.gameObject.CompareTag("Ground"))                         // SE o personagem estiver colidindo com um GameObject que possuir a tag Ground...
        {
            jump = true;                                                      // Pode pular (jump = true)
            heroAnim.SetBool("Jump", false);                       // Não está executando a animação de pulo
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 1, 1); // Mantem a cor do personagem no padrão
            //Debug.Log("colidiu");
        }
        if (collision.gameObject.CompareTag("Venom"))                        // Se o personagem colidir com um GameObject que possuir a tag Venom
        {
            gameAudio.PlayOneShot(sounds[3]);                                // Toca o som de dano (3) uma vez
            lifes = 0;                                                       // Altera a quantidade de vidas para 0
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 0, 0); // Altera a cor do personagem para vermelho
            lifesTxt.text = lifes.ToString();                                // Converte a quantidade de vidas para string
            GameOver();                                                      // Chama a função GameOver
        }
        if (collision.gameObject.CompareTag("Spike") || collision.gameObject.CompareTag("Blade")) // Se o personagem colidir com um GameObject que possuir a tag spike ou blade
        {
            gameAudio.PlayOneShot(sounds[3]);                                // Toca o som de dano (3) uma vez
            lifes -= 1;                                                      // Decrementa uma vida
            gameObject.GetComponentInChildren<SpriteRenderer>().color = new Color(1, 0, 0); // Altera a cor do personagem para vermelho
            lifesTxt.text = lifes.ToString();                                // Converte a quantidade de vidas para string
            if(lifes <= 0){                                                  // Se q quantidade de vidas for menor ou igual a 0
                GameOver();                                                  // Chama a função GameOver
            }

        }
        if (collision.gameObject.CompareTag("Flag"))                        // SE o personagem colidir com um GameObject com a tag Flag
        {
            //Debug.Log("flag");
            Win();                                                          // Chama a função Win
        }
        if (collision.gameObject.CompareTag("Button"))                      // SE o personagem colidir com um GameObject com a tag Button
        {
            gameAudio.PlayOneShot(sounds[0]);                               // Toca o som do toque no botão (0) uma vez 
            door.transform.eulerAngles = Vector3.Lerp(door.transform.eulerAngles, new Vector3(0, 80, 0), 0.9f); // Animação da porta
            door.gameObject.GetComponent<BoxCollider2D>().enabled = false;  // Desabilita o box collider da porta
        }

    }
    private void OnCollisionExit2D(Collision2D collision)                            // QUANDO A COLISÃO 2D ACABAR (PERSONAGEM PULOU)
    {
        if (collision.gameObject.CompareTag("Ground"))                               // SE a colisão estava acontecendo com um GameObject que possui a tag Ground...
        {
            jump = false;                                                            // Não pode pular novamente (jump = false) 
            // Debug.Log("pulou");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)                               // QUANDO ACONTECER UMA COLISÃO 2D
    {
        if(collision.gameObject.CompareTag("Star"))                                   // SE a tag do GameObject for star
        {
            stars += 1;                                                               // Incrementa 1 na quantidade de estrelas
            gameAudio.PlayOneShot(sounds[1]);                                         // Toca o som 1 (coleta de estrela) 
            starsTxt.text = stars.ToString();                                         // Propriedade texto do lifesTxt é igual à quantidade de vidas convertida em String (incrementada)                                    
            Destroy(collision.gameObject);                                            // Destrói o objeto (estrela)

        }
        if (collision.gameObject.CompareTag("Heart"))                                 // SE a tag do GameObject for heart 
        {
            lifes += 1;                                                               // Incrementa 1 na quantidade de corações
            gameAudio.PlayOneShot(sounds[2]);                                         // Toca o som 2 (coleta de coração)
            lifesTxt.text = lifes.ToString();                                         // Propriedade texto do starsTxt é igual à quantidade de estrelas convertida em String (incrementada)                  
                                                           // Destrói o objeto (coração)    

        }
    }
    public void Starting(){                                                            // Função pública - Iniciar o jogo
        start.gameObject.SetActive(false);                                             // Desativar o botão Start 
        Time.timeScale = 1;                                                            // O jogo é "despausado" (tempo começa a contar)
    }

    public void Restart(){                                                             // Função pública - Reiniciar o jogo
        restart.gameObject.SetActive(false);                                           // Desativar o botão Restart
        SceneManager.LoadScene(0);                                                     // Carregar a cena 0 
    }

    private void GameOver()                                                            // Função de Game Over
    {
        gameAudio.PlayOneShot(sounds[4]);                                              // Toca uma vez o som de Game Over (4)
        gameOverTxt.enabled = true;                                                    // Habilita o texto de Game Over
        restart.gameObject.SetActive(true);                                            // Habilita o botão de Restart
        Time.timeScale = 0;                                                            // Pausa novamente o jogo
    }

    private void Win()                                                                 // Função de Vitória
    {
        gameAudio.PlayOneShot(sounds[5]);                                              // Toca uma vez o som de Vitória (5)
        winTxt.enabled = true;                                                         // Habilita o texto de Vitória
        heroAnim.SetBool("Jump", true);                                     // Define a animação de Jump para true
        restart.gameObject.SetActive(true);                                            // Habilita o botão de Restart
        Time.timeScale = 0;                                                            // Pausa novamente o jogo
    }
}
