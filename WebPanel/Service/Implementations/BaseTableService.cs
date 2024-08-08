using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TryWebSite.Domain.Enum;
using WebPanel.DAL.Interfaces;
using WebPanel.DAL.Repositories;
using WebPanel.Domain.Entity;
using WebPanel.Domain.Response;
using WebPanel.Misc;
using WebPanel.Service.Interfaces;

namespace WebPanel.Service.Implementations
{
    public class BaseTableService : IBaseTableService
    {
        private readonly IBaseRepository<BaseTable> _basetableRepository;
        private readonly IBaseRepository<TableElement> _elementsRepository;
        private readonly IBaseRepository<FileModel> _filemodelRepository;
        public BaseTableService(IBaseRepository<BaseTable> basetableRepository, IBaseRepository<TableElement> elementsRepository, IBaseRepository<FileModel> fileModelRepository)
        {
            _basetableRepository = basetableRepository;
            _elementsRepository = elementsRepository;
            _filemodelRepository = fileModelRepository;
        }

        public async Task<IBaseResponse<BaseTable>> CreateTable(BaseTable viewModel)
        {
            var baseResponse = new BaseResponse<BaseTable>();
            try
            {
                var resource = new BaseTable()
                {
                    TableDisplayName = viewModel.TableDisplayName
                };

                await _basetableRepository.Add(resource);

                baseResponse.StatusCode = StatusCode.OK;
                baseResponse.Data = resource;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<BaseTable>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<BaseTable>> DeleteTable(int tableId)
        {
            var baseResponse = new BaseResponse<BaseTable>();
            try
            {
                var resource = await _basetableRepository.GetAll().FirstOrDefaultAsync(x => x.Id == tableId);
                if (resource == null)
                {
                    baseResponse.Description = "BaseTable not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                var tableElements = await _elementsRepository.GetAll().Where(x => x.TableId == tableId).ToListAsync();

                PanelFilesManager.DeleteFilePath(PanelFilesManager.MAIN_PATH + "/" + resource.Id);

                foreach (var element in tableElements)
                {
                    List<int> fileIds = new();

                    if (element.ConnectionsId != null)
                        fileIds.AddRange(JsonListConverter.GetListIntoString(element.ConnectionsId));
                    if (element.PictureId != null)
                        fileIds.AddRange(JsonListConverter.GetListIntoString(element.PictureId));
                    if (element.IllegalContentId != null)
                        fileIds.AddRange(JsonListConverter.GetListIntoString(element.IllegalContentId));

                    List<FileModel> filesList = _filemodelRepository.GetAll().Where(x => fileIds.Contains(x.Id)).ToList();

                    await _filemodelRepository.DeleteRange(filesList);
                }
                await _elementsRepository.DeleteRange(tableElements);

                await _basetableRepository.Delete(resource);

                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<BaseTable>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<List<BaseTable>>> GetAllTables()
        {
            var baseResponse = new BaseResponse<List<BaseTable>>();
            try
            {
                var resource = await _basetableRepository.GetAll().ToListAsync();
                if (resource == null)
                {
                    baseResponse.Description = "BaseTable not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<List<BaseTable>>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }

        public async Task<IBaseResponse<string>> GetTableName(int tableId)
        {
            var baseResponse = new BaseResponse<string>();
            try
            {
                var resource = await _basetableRepository.GetAll().FirstOrDefaultAsync(x => x.Id == tableId);
                if (resource == null)
                {
                    baseResponse.Description = "BaseTable not found";
                    baseResponse.StatusCode = StatusCode.ElementNotFound;
                    return baseResponse;
                }

                baseResponse.Data = resource.TableDisplayName;
                baseResponse.StatusCode = StatusCode.OK;
                return baseResponse;
            }
            catch (Exception ex)
            {
                return new BaseResponse<string>()
                {
                    Description = $"[GetResource] : {ex.Message}",
                    StatusCode = StatusCode.InternalServerError
                };
            }
        }
    }
}
