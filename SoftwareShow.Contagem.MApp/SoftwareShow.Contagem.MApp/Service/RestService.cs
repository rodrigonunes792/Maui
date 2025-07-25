using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using SoftwareShow.Contagem.MApp.Interfaces;
using SoftwareShow.Contagem.MApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SoftwareShow.Contagem.MApp.Service
{
    public class RestService : IRestService
    {
        private readonly HttpClient _httpClient;

        public RestService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        private async Task<string> GetApiBaseUrlAsync()
        {
            var apiUrl = await SecureStorage.GetAsync("api_url");
            if (string.IsNullOrEmpty(apiUrl))
            {
                throw new InvalidOperationException("URL da API não configurada.");
            }
            return apiUrl;
        }

        public async Task<Autenticacao> AutenticarAsync(string email, string senha, string versaoApp)
        {
            try
            {
                // Obter URL base da configuração
                var baseUrl = await GetApiBaseUrlAsync();

                // Construir endpoint
                string endPoint = "usuario/autenticar";
                var uri = new Uri(string.Concat(baseUrl.TrimEnd('/'), "/", endPoint));

                // Preparar dados da requisição
                var requestData = new
                {
                    Email = email,
                    Senha = senha,
                    VersaoApp = versaoApp
                };

                var json = JsonConvert.SerializeObject(requestData);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Fazer chamada HTTP
                var response = await _httpClient.PostAsync(uri, content);
                var responseJson = await response.Content.ReadAsStringAsync();

                // Processar resposta
                if (response.IsSuccessStatusCode)
                {
                    var autenticacao = JsonConvert.DeserializeObject<Autenticacao>(responseJson);
                    return autenticacao ?? CreateErrorResponse("Resposta inválida do servidor");
                }
                else
                {
                    // Tentar deserializar erro da API
                    try
                    {
                        var errorResponse = JsonConvert.DeserializeObject<Autenticacao>(responseJson);
                        return errorResponse ?? CreateErrorResponse(GetErrorByStatusCode(response.StatusCode));
                    }
                    catch
                    {
                        return CreateErrorResponse(GetErrorByStatusCode(response.StatusCode));
                    }
                }
            }
            catch (InvalidOperationException ex)
            {
                return CreateErrorResponse(ex.Message);
            }
            catch (HttpRequestException)
            {
                return CreateErrorResponse("Erro de conexão. Verifique sua internet e a URL da API.");
            }
            catch (TaskCanceledException)
            {
                return CreateErrorResponse("Timeout na requisição. Verifique a URL da API.");
            }
            catch (Exception)
            {
                return CreateErrorResponse("Erro inesperado. Tente novamente.");
            }
        }

        private static Autenticacao CreateErrorResponse(string mensagem)
        {
            return new Autenticacao
            {
                Autenticado = false,
                Mensagem = mensagem
            };
        }

        private static string GetErrorByStatusCode(System.Net.HttpStatusCode statusCode)
        {
            return statusCode switch
            {
                System.Net.HttpStatusCode.Unauthorized => "Usuário ou senha incorretos",
                System.Net.HttpStatusCode.BadRequest => "Dados de login inválidos",
                System.Net.HttpStatusCode.InternalServerError => "Erro interno do servidor",
                System.Net.HttpStatusCode.NotFound => "Serviço não encontrado. Verifique a URL da API",
                _ => "Erro ao autenticar"
            };
        }
    }
}
